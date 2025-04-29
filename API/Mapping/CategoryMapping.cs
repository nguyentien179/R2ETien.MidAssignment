using System;
using mid_assignment.Domain.Entities;
using mid_assignment.Presentations.DTO.Book;
using mid_assignment.Presentations.DTO.Category;

namespace mid_assignment.Mapping;

public static class CategoryMapping
{
    public static Category ToEntity(this CreateCategoryDTO dto)
    {
        return new Category { CategoryId = Guid.NewGuid(), Name = dto.Name };
    }

    public static Category ToEntity(this UpdateCategoryDTO dto)
    {
        return new Category { CategoryId = dto.CategoryId, Name = dto.Name };
    }

    public static CategoryDTO ToDTO(this Category category)
    {
        return new CategoryDTO(
            category.CategoryId,
            category.Name,
            category.Books?.Select(b => b.ToDTO()).ToList() ?? new List<BookDTO>()
        );
    }
}
