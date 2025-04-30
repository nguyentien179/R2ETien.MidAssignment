using System;
using mid_assignment.Domain.Enum;

namespace mid_assignment.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PasswordSalt { get; set; }
    public Gender Gender { get; set; }
    public Role Role { get; set; }

    public ICollection<BookBorrowingRequest>? BorrowingRequests { get; set; }
    public ICollection<BookBorrowingRequest>? ApprovedRequests { get; set; }
    public ICollection<BookReview> Reviews { get; set; } = new List<BookReview>();
}
