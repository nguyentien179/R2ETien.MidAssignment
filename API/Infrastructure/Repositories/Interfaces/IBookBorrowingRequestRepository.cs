using System;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.BookBorrowingRequest;

namespace mid_assignment.Infrastructure.Repositories.Interfaces;

public interface IBookBorrowingRequestRepository : IRepository<BookBorrowingRequest>
{
    Task<IEnumerable<BookBorrowingRequest>> GetByUserIdAsync(Guid userId);
}
