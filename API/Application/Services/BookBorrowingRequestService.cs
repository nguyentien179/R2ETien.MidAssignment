using System;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Common.Models;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Mapping;
using mid_assignment.Presentations.DTO.BookBorrowingRequest;
using mid_assignment.Presentations.DTO.BookBorrowingRequestDetails;

namespace mid_assignment.Application.Services;

public class BookBorrowingRequestService : IBookBorrowingRequestService
{
    private readonly IBookBorrowingRequestRepository _repository;
    private readonly IUserService _userService;
    private readonly IBookRepository _bookRepository;
    private readonly ApplicationDbContext _dbContext;

    public BookBorrowingRequestService(
        IBookBorrowingRequestRepository repository,
        IUserService userService,
        IBookRepository bookRepository,
        ApplicationDbContext dbContext
    )
    {
        _repository = repository;
        _userService = userService;
        _bookRepository = bookRepository;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<BookBorrowingRequestDTO>> GetAllAsync(
        List<Expression<Func<BookBorrowingRequest, bool>>>? filters = null,
        Func<IQueryable<BookBorrowingRequest>, IOrderedQueryable<BookBorrowingRequest>>? orderBy =
            null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        var bookBorrowingRequests = await _repository.GetAllAsync(
            filters,
            orderBy,
            includeProperties,
            pageNumber ??= 1,
            pageSize ??= 5
        );

        var bookBorrowingRequestDTOs = bookBorrowingRequests.Select(br => br.ToDTO());

        return bookBorrowingRequestDTOs;
    }

    public async Task<BookBorrowingRequestDTO?> GetByIdAsync(Guid id)
    {
        var request =
            await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.RequestNotFound);
        var currentUserId = _userService.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }
        if (request.RequestorId != currentUserId && !_userService.IsAdmin())
        {
            throw new UnauthorizedAccessException(ErrorMessages.NotRequestOwner);
        }
        return request?.ToDTO();
    }

    public async Task CreateAsync(CreateBookBorrowingRequestDTO dto)
    {
        if (dto.Details.Count > 5)
        {
            throw new InvalidOperationException("You can not borrow more than 5 books at a time");
        }
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await ValidateBooksForRequest(dto.Details);

            var currentUserId = _userService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
            }
            var updatedDto = dto with { RequestorId = currentUserId.Value };
            var request = updatedDto.ToEntity();

            await UpdateBookQuantities(dto.Details, -1);

            await _repository.AddAsync(request);
            await _repository.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw new Exception(ErrorMessages.Conflict);
        }
    }

    private async Task<bool> ValidateBooksForRequest(
        IEnumerable<CreateBookBorrowingRequestDetailsDTO> details
    )
    {
        var bookIds = details.Select(d => d.BookId).Distinct().ToList();
        var books = (await _bookRepository.GetBooksByIdsAsync(bookIds)).ToList();

        // Check if all books exist
        if (books.Count != bookIds.Count)
        {
            var missingIds = bookIds.Except(books.Select(b => b.BookId));
            throw new InvalidOperationException(ErrorMessages.BookNotFound);
        }

        foreach (var book in books)
        {
            var requestedCount = details.Count(d => d.BookId == book.BookId);
            if (book.Quantity < requestedCount)
            {
                throw new Exception($"Not enough quantity available for book '{book.Name}'");
            }
        }

        return true;
    }

    private async Task UpdateBookQuantities(
        IEnumerable<CreateBookBorrowingRequestDetailsDTO> details,
        int change
    )
    {
        var bookIds = details.Select(d => d.BookId).Distinct().ToList();

        var books = (await _bookRepository.GetBooksByIdsAsync(bookIds)).ToList();

        foreach (var book in books)
        {
            int changeAmount = change * details.Count(d => d.BookId == book.BookId);
            int newQuantity = book.Quantity + changeAmount;

            if (newQuantity < 0)
            {
                throw new InvalidOperationException($"Not enough copies of book {book.Name}.");
            }

            book.Quantity = newQuantity;
            _bookRepository.Update(book);
        }

        try
        {
            await _bookRepository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "Book inventory was modified by another request. Please try again."
            );
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity =
            await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.RequestNotFound);
        var currentUserId = _userService.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }
        if (entity.RequestorId != currentUserId && !_userService.IsAdmin())
        {
            throw new UnauthorizedAccessException(ErrorMessages.NotRequestOwner);
        }
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookBorrowingRequestDTO>> GetByUserIdAsync(Guid userId)
    {
        var currentUserId = _userService.GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }

        if (userId != currentUserId.Value && !_userService.IsAdmin())
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }

        var requests = await _repository.GetByUserIdAsync(userId);
        return requests.Select(br => br.ToDTO());
    }

    public async Task UpdateRequestStatusAsync(Guid requestId, RequestStatus requestStatus)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var request =
                await _repository.GetByIdAsync(requestId)
                ?? throw new KeyNotFoundException(ErrorMessages.RequestNotFound);

            var approverId = _userService.GetCurrentUserId();
            if (!approverId.HasValue)
            {
                throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
            }

            if (request.RequestStatus != RequestStatus.WAITING)
            {
                throw new InvalidOperationException(ErrorMessages.RequestProcessed);
            }

            if (!Enum.IsDefined(typeof(RequestStatus), requestStatus))
            {
                throw new ArgumentException("Invalid request status");
            }

            request.RequestStatus = requestStatus;
            request.ApproverId = approverId;

            switch (requestStatus)
            {
                case RequestStatus.APPROVED:
                    request.DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14));
                    break;

                case RequestStatus.REJECTED:
                    await ReturnBooksToInventory(request.BorrowingRequestDetails);
                    break;

                case RequestStatus.WAITING:
                    break;

                default:
                    throw new InvalidOperationException($"Unhandled status: {requestStatus}");
            }

            await _repository.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await transaction.RollbackAsync();
            throw new InvalidOperationException(
                "The request was modified by another user. Please reload and try again.",
                ex
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Failed to update request status: {ex.Message}");
        }
    }

    private async Task ReturnBooksToInventory(IEnumerable<BookBorrowingRequestDetails> details)
    {
        foreach (var detail in details)
        {
            if (detail.Book == null)
            {
                throw new InvalidOperationException(
                    $"Book reference missing for detail {detail.RequestDetailsId}"
                );
            }

            detail.Book.Quantity += 1;
            _bookRepository.Update(detail.Book);
        }

        await _bookRepository.SaveChangesAsync();
    }

    public async Task ExtendDueDate(Guid requestId)
    {
        var request =
            await _repository.GetByIdAsync(requestId)
            ?? throw new KeyNotFoundException(ErrorMessages.RequestNotFound);

        if (request.Extended == true)
        {
            throw new InvalidOperationException(ErrorMessages.DueDateExtended);
        }

        request.DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
        _repository.Update(request);
        await _repository.SaveChangesAsync();
    }
}
