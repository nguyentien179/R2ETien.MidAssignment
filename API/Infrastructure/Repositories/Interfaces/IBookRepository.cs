using System;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.Repositories.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds);
}
