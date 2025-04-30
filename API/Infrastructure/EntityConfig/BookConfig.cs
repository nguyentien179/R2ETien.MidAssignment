using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.EntityConfig;

public class BookConfig : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable(b =>
            b.HasCheckConstraint("CK_Book_Quantity_NonNegative", "[Quantity] >= 0")
        );
        builder
            .HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasKey(b => b.BookId);
        builder.Property(b => b.Author).IsRequired().HasMaxLength(50);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(40);
        builder.Property(b => b.Quantity).IsRequired();
        builder.Property(b => b.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasIndex(b => b.Name).IsUnique();
    }
}
