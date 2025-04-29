using mid_assignment.Presentations.DTO.BookBorrowingRequestDetails;

namespace mid_assignment.Presentations.DTO.BookBorrowingRequest;

public record class CreateBookBorrowingRequestDTO(
    Guid RequestorId,
    DateOnly RequestedDate,
    DateOnly DueDate,
    List<CreateBookBorrowingRequestDetailsDTO> Details
);