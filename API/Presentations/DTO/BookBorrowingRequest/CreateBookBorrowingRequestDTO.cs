using mid_assignment.Presentations.DTO.BookBorrowingRequestDetails;

namespace mid_assignment.Presentations.DTO.BookBorrowingRequest;

public record class CreateBookBorrowingRequestDTO(
    Guid RequestorId,
    List<CreateBookBorrowingRequestDetailsDTO> Details
);
