using System;

namespace mid_assignment.Domain.Entities;

public class BookReview
{
    public Guid BookReviewId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }
    public DateOnly ReviewDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
