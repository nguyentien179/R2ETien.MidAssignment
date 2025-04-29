using System;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.BookBorrowingRequest;

namespace mid_assignment.Mapping;

public static class BookBorrowingRequestMapping
{
    public static BookBorrowingRequest ToEntity(this CreateBookBorrowingRequestDTO dto)
    {
        var requestId = Guid.NewGuid();
        return new BookBorrowingRequest
        {
            RequestId = requestId,
            RequestorId = dto.RequestorId,
            RequestedDate = dto.RequestedDate,
            DueDate = dto.DueDate,
            RequestStatus = RequestStatus.WAITING,
            BorrowingRequestDetails = dto.Details.Select(d => d.ToEntity()).ToList(),
        };
    }

    public static BookBorrowingRequestDTO ToDTO(this BookBorrowingRequest request)
    {
        return new BookBorrowingRequestDTO(
            request.RequestId,
            request.RequestorId,
            request.Requestor?.Username ?? "Unknown",
            request.ApproverId,
            request.Approver?.Username,
            request.RequestedDate,
            request.DueDate,
            request.RequestStatus.ToString(),
            request.BorrowingRequestDetails?.Select(detail => detail.ToDTO()).ToList() ?? new()
        );
    }
}
