using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.EntityConfig;

public class BookReviewConfig : IEntityTypeConfiguration<BookReview>
{
    public void Configure(EntityTypeBuilder<BookReview> builder)
    {
        builder.ToTable("BookReview");
        builder.HasIndex(br => new { br.BookId, br.UserId }).IsUnique();
    }
}
