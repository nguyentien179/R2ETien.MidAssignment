using System;
using Microsoft.EntityFrameworkCore;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Infrastructure.Repositories.Interfaces;

namespace mid_assignment.Infrastructure.Repositories;

public class BookReviewRepository : GeneralRepository<BookReview>, IBookReviewRepository
{
    private readonly ApplicationDbContext _context;

    public BookReviewRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookReview>> GetAllByBookIdAsync(Guid bookId)
    {
        return await _context
            .BookReviews.Where(br => br.BookId == bookId)
            .Include(br => br.User)
            .ToListAsync();
    }
}
