namespace mid_assignment.Presentations.DTO.BookReview;

public record class UpdateBookReviewDTO(
    Guid ReviewId,
    int Rating,
    string? Comment,
    Guid BookId,
    Guid UserId
);
