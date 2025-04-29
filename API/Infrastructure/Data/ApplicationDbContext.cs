using System;
using Microsoft.EntityFrameworkCore;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.EntityConfig;

namespace mid_assignment.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookBorrowingRequest> BookBorrowingRequests { get; set; }
    public DbSet<BookBorrowingRequestDetails> BookBorrowingRequestDetails { get; set; }
    public DbSet<BookReview> BookReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfig());
        modelBuilder.ApplyConfiguration(new BookConfig());
        modelBuilder.ApplyConfiguration(new CategoryConfig());
        modelBuilder.ApplyConfiguration(new BookBorrowingRequestConfig());
        modelBuilder.ApplyConfiguration(new BookBorrowingRequestDetailsConfig());
        modelBuilder.ApplyConfiguration(new BookReviewConfig());

        modelBuilder
            .Entity<Category>()
            .HasData(
                new Category
                {
                    CategoryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Fiction",
                },
                new Category
                {
                    CategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Science",
                },
                new Category
                {
                    CategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "History",
                }
            );
    }
}
