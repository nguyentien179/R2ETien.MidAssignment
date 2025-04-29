namespace mid_assignment.Presentations.DTO.Book;

public record class BookDTO(
    Guid BookId,
    string ImageUrl,
    string Name,
    string Author,
    Guid CategoryId,
    string CategoryName,
    int Quantity
);
