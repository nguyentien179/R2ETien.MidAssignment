using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.EntityConfig;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(30);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(40);
        builder.Property(u => u.Role).IsRequired().HasConversion<string>();
        builder.Property(u => u.Gender).IsRequired().HasConversion<string>();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();

        builder
            .HasMany(u => u.BorrowingRequests)
            .WithOne(b => b.Requestor)
            .HasForeignKey(b => b.RequestorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(u => u.ApprovedRequests)
            .WithOne(b => b.Approver)
            .HasForeignKey(b => b.ApproverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
