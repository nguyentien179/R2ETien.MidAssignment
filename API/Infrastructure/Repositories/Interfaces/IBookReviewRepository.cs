using System;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.Repositories.Interfaces;

public interface IBookReviewRepository : IRepository<BookReview>
{
    Task<IEnumerable<BookReview>> GetAllByBookIdAsync(Guid bookId);
}
