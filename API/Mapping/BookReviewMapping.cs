using System;
using mid_assignment.Domain.Entities;
using mid_assignment.Presentations.DTO.BookReview;

namespace mid_assignment.Mapping;

public static class BookReviewMapping
{
    public static BookReviewDTO ToDTO(this BookReview bookReview)
    {
        return new BookReviewDTO(
            bookReview.BookReviewId,
            bookReview.Rating,
            bookReview.Comment ?? string.Empty,
            bookReview.ReviewDate,
            bookReview.User.Username
        );
    }

    public static BookReview ToEntity(this CreateBookReviewDTO dto, Guid bookId, Guid userId)
    {
        return new BookReview
        {
            BookReviewId = Guid.NewGuid(),
            Rating = dto.Rating,
            Comment = dto.Comment,
            ReviewDate = DateOnly.FromDateTime(DateTime.Today),
            BookId = bookId,
            UserId = userId,
        };
    }

    public static BookReview ToEntity(this UpdateBookReviewDTO bookReviewDTO)
    {
        return new BookReview
        {
            BookReviewId = bookReviewDTO.ReviewId,
            Rating = bookReviewDTO.Rating,
            Comment = bookReviewDTO.Comment,
            ReviewDate = DateOnly.FromDateTime(DateTime.Today),
            BookId = bookReviewDTO.BookId,
            UserId = bookReviewDTO.UserId,
        };
    }
}
