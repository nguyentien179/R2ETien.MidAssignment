namespace mid_assignment.Presentations.DTO.BookReview;

public record class BookReviewDTO(
    Guid BookReviewId,
    int Rating,
    string Comment,
    DateOnly ReviewDate,
    string Username
);
