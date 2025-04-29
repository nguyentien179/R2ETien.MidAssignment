using System;
using mid_assignment.Presentations.DTO.BookReview;

namespace mid_assignment.Application.Interfaces;

public interface IBookReviewService
{
    Task<IEnumerable<BookReviewDTO>> GetAllByBookIdAsync(Guid bookId);
    Task<BookReviewDTO?> GetByIdAsync(Guid bookReviewId);
    Task CreateAsync(CreateBookReviewDTO dto, Guid bookId);
    Task UpdateAsync(Guid bookReviewId, UpdateBookReviewDTO dto);
    Task DeleteAsync(Guid bookReviewId);
}
