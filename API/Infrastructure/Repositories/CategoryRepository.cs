using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using mid_assignment.Application.Common.Models;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Infrastructure.Helper;
using mid_assignment.Infrastructure.Repositories.Interfaces;

namespace mid_assignment.Infrastructure.Repositories;

public class CategoryRepository : GeneralRepository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public override async Task<IEnumerable<Category>> GetAllAsync(
        List<Expression<Func<Category, bool>>>? filters = null,
        Func<IQueryable<Category>, IOrderedQueryable<Category>>? orderBy = null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        IQueryable<Category> query = _context.Categories.Include(c => c.Books);

        query = QueryHelper.ApplyFilters(query, filters);

        // Apply includes
        query = QueryHelper.ApplyIncludes(query, includeProperties);

        // Apply sorting
        query = QueryHelper.ApplySorting(query, orderBy);

        return await query.AsNoTracking().ToListAsync();
    }

    public override async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context
            .Categories.Include(c => c.Books)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == id);
    }

    public async Task<bool> CategoryNameExistAsync(string categoryName)
    {
        return await _context.Categories.AnyAsync(c => c.Name == categoryName);
    }
}
