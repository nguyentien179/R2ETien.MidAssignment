using System;
using System.Linq.Expressions;
using mid_assignment.Application.Common.Models;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.BookBorrowingRequest;

namespace mid_assignment.Application.Interfaces;

public interface IBookBorrowingRequestService
{
    Task CreateAsync(CreateBookBorrowingRequestDTO dto);
    Task<BookBorrowingRequestDTO?> GetByIdAsync(Guid id);
    Task<IEnumerable<BookBorrowingRequestDTO>> GetAllAsync(
        List<Expression<Func<BookBorrowingRequest, bool>>>? filters = null,
        Func<IQueryable<BookBorrowingRequest>, IOrderedQueryable<BookBorrowingRequest>>? orderBy =
            null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    );
    Task DeleteAsync(Guid id);
    Task<IEnumerable<BookBorrowingRequestDTO>> GetByUserIdAsync(Guid userId);
    Task UpdateRequestStatusAsync(Guid requestId, RequestStatus requestStatus);
    Task ExtendDueDate(Guid requestId);
}
