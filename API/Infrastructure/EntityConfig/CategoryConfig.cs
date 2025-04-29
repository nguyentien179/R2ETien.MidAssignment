using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.EntityConfig;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");
        builder.Property(c => c.Name).IsRequired().HasMaxLength(40);
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasKey(c => c.CategoryId);
    }
}
