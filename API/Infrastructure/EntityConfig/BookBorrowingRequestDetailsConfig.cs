using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.EntityConfig;

public class BookBorrowingRequestDetailsConfig
    : IEntityTypeConfiguration<BookBorrowingRequestDetails>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequestDetails> builder)
    {
        builder.ToTable("BookBorrowingRequestDetails");
        builder.HasKey(brd => brd.RequestDetailsId);
        builder
            .HasOne(brd => brd.Request)
            .WithMany(br => br.BorrowingRequestDetails)
            .HasForeignKey(brd => brd.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(brd => brd.Book)
            .WithMany(b => b.BorrowingRequestDetails)
            .HasForeignKey(brd => brd.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
