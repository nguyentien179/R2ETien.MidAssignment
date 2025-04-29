using System;
using mid_assignment.Domain.Entities;
using mid_assignment.Presentations.DTO.BookBorrowingRequestDetails;

namespace mid_assignment.Mapping;

public static class BookBorrowingRequestDetailsMapping
{
    public static BookBorrowingRequestDetailsDTO ToDTO(this BookBorrowingRequestDetails detail)
    {
        return new BookBorrowingRequestDetailsDTO(
            detail.RequestDetailsId,
            detail.BookId,
            detail.Book?.Name ?? "Unknown"
        );
    }

    public static BookBorrowingRequestDetails ToEntity(
        this CreateBookBorrowingRequestDetailsDTO dto
    )
    {
        return new BookBorrowingRequestDetails
        {
            RequestDetailsId = Guid.NewGuid(),
            BookId = dto.BookId,
        };
    }
}
