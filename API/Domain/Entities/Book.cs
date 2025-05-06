using System;
using System.ComponentModel.DataAnnotations;
using mid_assignment.Domain.Enum;

namespace mid_assignment.Domain.Entities;

public class Book
{
    public Guid BookId { get; set; } = Guid.NewGuid();
    public required string ImageUrl { get; set; }
    public required string Name { get; set; }
    public required string Author { get; set; }

    public required int Quantity { get; set; }
    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<BookBorrowingRequestDetails>? BorrowingRequestDetails { get; set; }
    public ICollection<BookReview> Reviews { get; set; } = new List<BookReview>();
    public RequestStatus RequestStatus { get; internal set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
