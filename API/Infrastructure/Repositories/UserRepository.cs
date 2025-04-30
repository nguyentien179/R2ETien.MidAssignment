using System;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Infrastructure.Repositories.Interfaces;

namespace mid_assignment.Infrastructure.Repositories;

public class UserRepository : GeneralRepository<User>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _context
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    public async Task RegisterAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}
