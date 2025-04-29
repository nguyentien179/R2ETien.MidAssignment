using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.BookBorrowingRequestDetails;

namespace mid_assignment.Presentations.DTO.BookBorrowingRequest;

public record class UpdateBookBorrowingRequestDTO(
    DateOnly DueDate,
    List<UpdateBookBorrowingRequestDetailsDTO> Details
);
