using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.EntityConfig;

public class BookBorrowingRequestConfig : IEntityTypeConfiguration<BookBorrowingRequest>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequest> builder)
    {
        builder.ToTable("BookBorrowingRequest");
        builder
            .HasOne(br => br.Requestor)
            .WithMany(u => u.BorrowingRequests)
            .HasForeignKey(br => br.RequestorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(br => br.Approver)
            .WithMany(u => u.ApprovedRequests)
            .HasForeignKey(br => br.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(br => br.RequestStatus)
            .HasConversion<string>()
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(br => br.RequestedDate).IsRequired();
        builder.Property(br => br.DueDate).IsRequired();
        builder.HasKey(br => br.RequestId);
    }
}
