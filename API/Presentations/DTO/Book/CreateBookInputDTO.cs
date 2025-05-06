using System;

namespace mid_assignment.Presentations.DTO.Book;

public record class CreateBookInputDTO(
    string Name,
    string Author,
    Guid CategoryId,
    int Quantity,
    IFormFile Image
);
