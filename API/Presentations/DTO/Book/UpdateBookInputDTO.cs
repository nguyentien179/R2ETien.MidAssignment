namespace mid_assignment.Presentations.DTO.Book;

public record class UpdateBookInputDTO(
    string Name,
    string Author,
    Guid CategoryId,
    int Quantity,
    IFormFile Image
);
