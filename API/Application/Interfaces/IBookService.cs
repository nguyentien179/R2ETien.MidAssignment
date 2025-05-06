using System;
using System.Linq.Expressions;
using mid_assignment.Domain.Entities;
using mid_assignment.Presentations.DTO.Book;

namespace mid_assignment.Application.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookDTO>> GetAllAsync(
        List<Expression<Func<Book, bool>>>? filters = null,
        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    );
    Task<BookDTO?> GetByIdAsync(Guid id);
    Task CreateAsync(CreateBookInputDTO dto);
    Task UpdateAsync(UpdateBookInputDTO dto, Guid id);
    Task DeleteAsync(Guid id);
}
