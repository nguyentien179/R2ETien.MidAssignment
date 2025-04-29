namespace mid_assignment.Presentations.DTO.Book;

public record class UpdateBookDTO(
    string Name,
    string Author,
    Guid CategoryId,
    int Quantity,
    string ImageUrl
);
