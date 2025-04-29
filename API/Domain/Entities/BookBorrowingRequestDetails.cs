using System;

namespace mid_assignment.Domain.Entities;

public class BookBorrowingRequestDetails
{
    public Guid RequestDetailsId { get; set; }
    public Guid RequestId { get; set; }
    public Guid BookId { get; set; }
    public BookBorrowingRequest Request { get; set; } = default!;
    public Book Book { get; set; } = default!;
}
