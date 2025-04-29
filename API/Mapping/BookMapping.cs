using System;
using mid_assignment.Domain.Entities;
using mid_assignment.Presentations.DTO.Book;

namespace mid_assignment.Mapping;

public static class BookMapping
{
    public static Book ToEntity(this CreateBookDTO dto)
    {
        return new Book
        {
            BookId = Guid.NewGuid(),
            Name = dto.Name,
            Author = dto.Author,
            CategoryId = dto.CategoryId,
            Quantity = dto.Quantity,
            ImageUrl = dto.ImageUrl,
        };
    }

    public static Book ToEntity(this UpdateBookDTO dto)
    {
        return new Book
        {
            Name = dto.Name,
            Author = dto.Author,
            CategoryId = dto.CategoryId,
            Quantity = dto.Quantity,
            ImageUrl = dto.ImageUrl,
        };
    }

    public static BookDTO ToDTO(this Book book)
    {
        return new BookDTO(
            book.BookId,
            book.ImageUrl,
            book.Name,
            book.Author,
            book.CategoryId,
            book.Category?.Name ?? "N/A",
            book.Quantity
        );
    }
}
