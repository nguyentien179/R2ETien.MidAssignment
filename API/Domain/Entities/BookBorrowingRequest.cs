using System;
using System.ComponentModel.DataAnnotations;
using mid_assignment.Domain.Enum;

namespace mid_assignment.Domain.Entities;

public class BookBorrowingRequest
{
    public Guid RequestId { get; set; }
    public Guid RequestorId { get; set; }
    public DateOnly RequestedDate { get; set; }
    public bool Extended { get; set; } = false;
    public RequestStatus RequestStatus { get; set; }
    public Guid? ApproverId { get; set; }
    public DateOnly DueDate { get; set; }

    public User Requestor { get; set; } = null!;
    public User? Approver { get; set; }
    public required ICollection<BookBorrowingRequestDetails> BorrowingRequestDetails { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }
}
