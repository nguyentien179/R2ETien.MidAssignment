using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Mapping;
using mid_assignment.Presentations.DTO.Category;

namespace mid_assignment.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task CreateAsync(CreateCategoryDTO dto)
    {
        var existingCategory = await _categoryRepository.GetAllAsync();

        if (existingCategory.Any(c => c.Name.ToLower() == dto.Name.ToLower()))
        {
            throw new InvalidOperationException(ErrorMessages.CategoryNameExist);
        }
        await _categoryRepository.AddAsync(dto.ToEntity());
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category =
            await _categoryRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.NotFound);
        _categoryRepository.Delete(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryDTO>> GetAllAsync(
        List<Expression<Func<Category, bool>>>? filters = null,
        Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        var categories = await _categoryRepository.GetAllAsync(
            filters,
            orderBy,
            includeProperties,
            pageNumber ??= 1,
            pageSize ??= 5
        );
        return categories.Select(b => b.ToDTO());
    }

    public async Task<CategoryDTO?> GetByIdAsync(Guid id)
    {
        var category =
            await _categoryRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.NotFound);
        return category.ToDTO();
    }

    public async Task UpdateAsync(UpdateCategoryDTO dto, Guid id)
    {
        var category =
            await _categoryRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.NotFound);
        var existingCategory = await _categoryRepository.GetAllAsync();

        if (existingCategory.Any(c => c.Name.ToLower() == dto.Name.ToLower()))
        {
            throw new InvalidOperationException(ErrorMessages.CategoryNameExist);
        }
        _categoryRepository.Update(dto.ToEntity());
        await _categoryRepository.SaveChangesAsync();
    }
}
