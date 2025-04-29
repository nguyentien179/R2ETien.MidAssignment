using System;
using System.Linq.Expressions;
using mid_assignment.Domain.Entities;
using mid_assignment.Presentations.DTO.Category;

namespace mid_assignment.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDTO>> GetAllAsync(
        List<Expression<Func<Category, bool>>>? filters = null,
        Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    );
    Task<CategoryDTO?> GetByIdAsync(Guid id);
    Task CreateAsync(CreateCategoryDTO dto);
    Task UpdateAsync(UpdateCategoryDTO dto, Guid id);
    Task DeleteAsync(Guid id);
}
