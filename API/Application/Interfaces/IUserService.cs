using System;
using System.Linq.Expressions;
using mid_assignment.Domain.Entities;
using mid_assignment.Presentations.DTO.Token;
using mid_assignment.Presentations.DTO.User;

namespace mid_assignment.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllAsync(
        List<Expression<Func<User, bool>>>? filters = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    );
    Task<UserDTO?> GetByIdAsync(Guid id);
    Task<(User User, string Token, string RefreshToken)?> LoginAsync(UserLoginDTO dto);
    Task RegisterAsync(UserRegisterDTO dto);
    Task LogoutAsync(Guid userId);
    Task<TokenResponseDTO?> RefreshTokenAsync(string token);
    Guid? GetCurrentUserId();
    bool IsAdmin();
    Task<UserDTO> GetCurrentUserAsync();
    Task UpdateAsync(UpdateUserDTO dto, Guid id);
    Task DeleteAsync(Guid id);
}
