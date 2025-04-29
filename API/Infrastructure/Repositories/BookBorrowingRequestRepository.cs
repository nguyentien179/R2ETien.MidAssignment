using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Common.Models;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Infrastructure.Helper;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Presentations.DTO.BookBorrowingRequest;

namespace mid_assignment.Infrastructure.Repositories;

public class BookBorrowingRequestRepository
    : GeneralRepository<BookBorrowingRequest>,
        IBookBorrowingRequestRepository
{
    private readonly ApplicationDbContext _context;

    public BookBorrowingRequestRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public override async Task<IEnumerable<BookBorrowingRequest>> GetAllAsync(
        List<Expression<Func<BookBorrowingRequest, bool>>>? filters = null,
        Func<IQueryable<BookBorrowingRequest>, IOrderedQueryable<BookBorrowingRequest>>? orderBy =
            null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        IQueryable<BookBorrowingRequest> query = _context
            .BookBorrowingRequests.Include(r => r.Requestor)
            .Include(r => r.Approver)
            .Include(r => r.BorrowingRequestDetails)
            .ThenInclude(brd => brd.Book);

        // Apply filters
        query = QueryHelper.ApplyFilters(query, filters);

        // Apply includes
        query = QueryHelper.ApplyIncludes(query, includeProperties);

        // Apply sorting
        query = QueryHelper.ApplySorting(query, orderBy);

        return await query.AsNoTracking().ToListAsync();
    }

    public override async Task<BookBorrowingRequest?> GetByIdAsync(Guid id)
    {
        return await _context
            .BookBorrowingRequests.Include(r => r.Requestor)
            .Include(r => r.Approver)
            .Include(r => r.BorrowingRequestDetails)
            .ThenInclude(d => d.Book)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RequestId == id);
    }

    public async Task<IEnumerable<BookBorrowingRequest>> GetByUserIdAsync(Guid userId)
    {
        return await _context
            .BookBorrowingRequests.Where(br => br.RequestorId == userId)
            .AsNoTracking()
            .ToListAsync();
    }
}
