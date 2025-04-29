using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using mid_assignment.Application.Common.Models;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Infrastructure.Helper;
using mid_assignment.Infrastructure.Repositories.Interfaces;

namespace mid_assignment.Infrastructure.Repositories;

public class BookRepository : GeneralRepository<Book>, IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public override async Task<IEnumerable<Book>> GetAllAsync(
        List<Expression<Func<Book, bool>>>? filters = null,
        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        IQueryable<Book> query = _context.Books.Include(b => b.Category);

        query = QueryHelper.ApplyFilters(query, filters);

        // Apply includes
        query = QueryHelper.ApplyIncludes(query, includeProperties);

        // Apply sorting
        query = QueryHelper.ApplySorting(query, orderBy);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds)
    {
        var distinctBookIds = bookIds.Distinct().ToList();

        if (!distinctBookIds.Any())
        {
            return new List<Book>();
        }

        return await _context
            .Books.AsNoTracking()
            .Where(b => distinctBookIds.Contains(b.BookId))
            .ToListAsync();
    }

    public override async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _context
            .Books.Include(b => b.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookId == id);
    }
}
