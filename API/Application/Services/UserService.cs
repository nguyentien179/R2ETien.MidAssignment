using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Mapping;
using mid_assignment.Presentations.DTO.Token;
using mid_assignment.Presentations.DTO.User;

namespace mid_assignment.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(
        IUserRepository userRepository,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<(User User, string Token, string RefreshToken)?> LoginAsync(UserLoginDTO dto)
    {
        var user = await _userRepository.GetUserByUsernameOrEmailAsync(dto.UsernameOrEmail);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!VerifyPassword(dto.Password, user.Password, user.PasswordSalt))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        _userRepository.Update(user);

        string token = GenerateJwtToken(user);
        return (user, token, user.RefreshToken);
    }

    public async Task RegisterAsync(UserRegisterDTO dto)
    {
        var existingUser = await _userRepository.GetAllAsync();
        if (
            existingUser.Any(u =>
                u.Username.Equals(dto.Username, StringComparison.CurrentCultureIgnoreCase)
            )
        )
        {
            throw new InvalidOperationException(ErrorMessages.UsernameTaken);
        }
        if (
            existingUser.Any(u =>
                u.Email.Equals(dto.Email, StringComparison.CurrentCultureIgnoreCase)
            )
        )
        {
            throw new InvalidOperationException(ErrorMessages.EmailTaken);
        }
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        string saltString = Convert.ToBase64String(salt);

        string hashedPassword = HashPassword(dto.Password, saltString);

        var user = dto.ToEntity(hashedPassword, saltString);
        await _userRepository.RegisterAsync(user);
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new Exception(ErrorMessages.UserNotFound);
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
    }

    private string HashPassword(string password, string salt)
    {
        int keySize = 256 / 8;
        int iterations = 10000;
        byte[] saltBytes = Convert.FromBase64String(salt);

        using (
            var deriveBytes = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                iterations,
                HashAlgorithmName.SHA256
            )
        )
        {
            byte[] hashBytes = deriveBytes.GetBytes(keySize);
            return Convert.ToBase64String(hashBytes);
        }
    }

    private bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        string hashedPassword = HashPassword(password, storedSalt);
        return hashedPassword == storedHash;
    }

    private string GenerateJwtToken(User user)
    {
        var keyString = _configuration["JwtSettings:SecretKey"] ?? throw new Exception("no key");

        var key = Encoding.ASCII.GetBytes(keyString);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.Id.ToString()),
                }
            ),
            Expires = DateTime.UtcNow.AddHours(
                Convert.ToInt32(_configuration["JwtSettings:ExpirationHours"] ?? "1")
            ),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.ToDTO());
    }

    public async Task<UserDTO?> GetByIdAsync(Guid id)
    {
        var user =
            await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.UserNotFound);
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }
        if (user.Id != currentUserId && !IsAdmin())
        {
            throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        }
        return user.ToDTO();
    }

    public Guid? GetCurrentUserId()
    {
        var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue("UserId");
        return Guid.TryParse(userIdString, out var userId) ? userId : (Guid?)null;
    }

    public bool IsAdmin()
    {
        var role = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
        return role == Role.ADMIN.ToString();
    }

    public async Task<IEnumerable<UserDTO>> GetAllAsync(
        List<Expression<Func<User, bool>>>? filters = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        string? includeProperties = null,
        int? pageNumber = null,
        int? pageSize = null
    )
    {
        var users = await _userRepository.GetAllAsync(
            filters,
            orderBy,
            includeProperties,
            pageNumber ??= 1,
            pageSize ??= 5
        );

        var userDTOs = users.Select(br => br.ToDTO());

        return userDTOs;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<TokenResponseDTO?> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new UnauthorizedAccessException(ErrorMessages.InvalidReFreshToken);

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return new TokenResponseDTO(newAccessToken, newRefreshToken);
    }

    public async Task<UserDTO> GetCurrentUserAsync()
    {
        var userId =
            GetCurrentUserId() ?? throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new UnauthorizedAccessException(ErrorMessages.Forbidden);
        return user.ToDTO();
    }

    public Task UpdateAsync(UpdateUserDTO dto, Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user =
            await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException(ErrorMessages.UserNotFound);
        _userRepository.Delete(user);
        await _userRepository.SaveChangesAsync();
    }
}
