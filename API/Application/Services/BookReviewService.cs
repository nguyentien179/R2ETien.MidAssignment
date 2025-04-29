using System;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Interfaces;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Mapping;
using mid_assignment.Presentations.DTO.BookReview;

namespace mid_assignment.Application.Services;

public class BookReviewService : IBookReviewService
{
    private readonly IBookReviewRepository _repository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserService _userService;

    public BookReviewService(
        IBookReviewRepository repository,
        IBookRepository bookRepository,
        IUserService userService
    )
    {
        _repository = repository;
        _bookRepository = bookRepository;
        _userService = userService;
    }

    public async Task CreateAsync(CreateBookReviewDTO dto, Guid bookId)
    {
        var userId = _userService.GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }
        var review = dto.ToEntity(bookId, userId.Value);
        await _repository.AddAsync(review);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid bookReviewId)
    {
        var existingReview =
            await _repository.GetByIdAsync(bookReviewId)
            ?? throw new KeyNotFoundException(ErrorMessages.ReviewNotFound);
        var userId = _userService.GetCurrentUserId();
        if (existingReview.UserId != userId && !_userService.IsAdmin())
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }
        _repository.Delete(existingReview);
        await _repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookReviewDTO>> GetAllByBookIdAsync(Guid bookId)
    {
        var book =
            await _bookRepository.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException(ErrorMessages.BookNotFound);
        var reviews = await _repository.GetAllByBookIdAsync(bookId);
        return reviews.Select(r => r.ToDTO());
    }

    public async Task<BookReviewDTO?> GetByIdAsync(Guid bookReviewId)
    {
        var existingReview =
            await _repository.GetByIdAsync(bookReviewId)
            ?? throw new KeyNotFoundException(ErrorMessages.ReviewNotFound);
        return existingReview.ToDTO();
    }

    public async Task UpdateAsync(Guid bookReviewId, UpdateBookReviewDTO dto)
    {
        var existingReview =
            await _repository.GetByIdAsync(bookReviewId)
            ?? throw new KeyNotFoundException(ErrorMessages.ReviewNotFound);
        var userId = _userService.GetCurrentUserId();
        if (existingReview.UserId != userId)
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }

        existingReview.Rating = dto.Rating;
        existingReview.Comment = dto.Comment;
        existingReview.ReviewDate = DateOnly.FromDateTime(DateTime.Today);

        _repository.Update(existingReview);
        await _repository.SaveChangesAsync();
    }
}
