using System;
using System.Linq.Expressions;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Mapping;
using mid_assignment.Presentations.DTO.Book;

namespace mid_assignment.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IImageUploader _imageUploader;

    public BookService(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IImageUploader imageUploader
    )
    {
        _bookRepository = bookRepository;
        _categoryRepository = categoryRepository;
        _imageUploader = imageUploader;
    }

    public async Task CreateAsync(CreateBookInputDTO dto)
    {
        var books = await _bookRepository.GetAllAsync();
        if (books.Any(b => b.Name.Equals(dto.Name, StringComparison.CurrentCultureIgnoreCase)))
        {
            throw new InvalidOperationException(ErrorMessages.BookNameTaken);
        }
        var category =
            await _categoryRepository.GetByIdAsync(dto.CategoryId)
            ?? throw new KeyNotFoundException(ErrorMessages.CategoryNotFound);
        var imageUrl = await _imageUploader.UploadImageAsync(dto.Image);
        var fullDto = new CreateBookDTO(
            dto.Name,
            dto.Author,
            dto.CategoryId,
            dto.Quantity,
            dto.Image,
            imageUrl!
        );
        await _bookRepository.AddAsync(fullDto.ToEntity());
        await _bookRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var book =
            await _bookRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.BookNotFound);
        _bookRepository.Delete(book);
        await _bookRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<BookDTO>> GetAllAsync(
        List<Expression<Func<Book, bool>>>? filters = null,
        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        var books = await _bookRepository.GetAllAsync(
            filters,
            orderBy,
            includeProperties,
            pageNumber ??= 1,
            pageSize ??= 5
        );
        return books.Select(b => b.ToDTO());
    }

    public async Task<BookDTO?> GetByIdAsync(Guid id)
    {
        var book =
            await _bookRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.BookNotFound);
        return book.ToDTO();
    }

    public async Task UpdateAsync(UpdateBookDTO dto, Guid id)
    {
        Console.WriteLine($"Incoming CategoryId: {dto.CategoryId}");
        var book =
            await _bookRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.BookNotFound);

        var books = await _bookRepository.GetAllAsync();
        if (
            books.Any(b =>
                b.BookId != id && b.Name.Equals(dto.Name, StringComparison.CurrentCultureIgnoreCase)
            )
        )
        {
            throw new InvalidOperationException(ErrorMessages.BookNameTaken);
        }

        var category =
            await _categoryRepository.GetByIdAsync(dto.CategoryId)
            ?? throw new KeyNotFoundException(ErrorMessages.CategoryNotFound);

        if (dto.Image != null)
        {
            var imageUrl = await _imageUploader.UploadImageAsync(dto.Image);
            book.ImageUrl = imageUrl!;
        }

        book.Name = dto.Name;
        book.Author = dto.Author;
        book.Category = null;
        book.CategoryId = dto.CategoryId;
        book.Quantity = dto.Quantity;

        _bookRepository.Update(book);
        await _bookRepository.SaveChangesAsync();
    }
}
