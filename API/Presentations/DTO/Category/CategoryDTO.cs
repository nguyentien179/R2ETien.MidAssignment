using mid_assignment.Presentations.DTO.Book;

namespace mid_assignment.Presentations.DTO.Category;

public record class CategoryDTO(Guid CategoryId, string Name, List<BookDTO> Books);
