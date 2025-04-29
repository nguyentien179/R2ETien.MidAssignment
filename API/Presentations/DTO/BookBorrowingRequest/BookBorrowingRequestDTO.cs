using mid_assignment.Presentations.DTO.BookBorrowingRequestDetails;

namespace mid_assignment.Presentations.DTO.BookBorrowingRequest;

public record class BookBorrowingRequestDTO
(
    Guid RequestId,
    Guid RequestorId,
    string RequestorName,
    Guid? ApproverId,
    string? ApproverName,
    DateOnly RequestedDate,
    DateOnly DueDate,
    string RequestStatus,
    List<BookBorrowingRequestDetailsDTO> Details
);
