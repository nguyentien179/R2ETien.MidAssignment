namespace mid_assignment.Presentations.DTO.BookBorrowingRequestDetails;

public record class BookBorrowingRequestDetailsDTO(
    Guid RequestDetailsId,
    Guid BookId,
    string BookName
);
