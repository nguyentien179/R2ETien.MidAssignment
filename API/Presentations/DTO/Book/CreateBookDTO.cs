namespace mid_assignment.Presentations.DTO.Book;

public record class CreateBookDTO(
    string Name,
    string Author,
    Guid CategoryId,
    int Quantity,
    IFormFile Image,
    string? ImageUrl
);
