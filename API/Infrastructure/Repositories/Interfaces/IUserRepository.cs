using System;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task RegisterAsync(User user);
}
