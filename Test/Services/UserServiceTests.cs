using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using mid_assignment.Application.Services;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Presentations.DTO.User;
using Moq;

namespace Test.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _userService = new UserService(
            _userRepositoryMock.Object,
            _configurationMock.Object,
            _httpContextAccessorMock.Object
        );
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

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsUserAndTokens()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            Role = Role.USER,
            PasswordSalt = "testSalt",
            Password = Convert.ToBase64String(
                new Rfc2898DeriveBytes(
                    "password",
                    Convert.FromBase64String("testSalt"),
                    10000,
                    HashAlgorithmName.SHA256
                ).GetBytes(32)
            )
        };

        var loginDto = new UserLoginDTO("testuser", "password");

        _userRepositoryMock
            .Setup(r => r.GetUserByUsernameOrEmailAsync(loginDto.UsernameOrEmail))
            .ReturnsAsync(user);

        _configurationMock
            .Setup(c => c["JwtSettings:SecretKey"])
            .Returns("THIS_IS_A_SECRET_KEY_123456789012345678901234");

        _configurationMock.Setup(c => c["JwtSettings:ExpirationHours"]).Returns("1");

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result?.User);
        Assert.False(string.IsNullOrEmpty(result?.Token));
        Assert.False(string.IsNullOrEmpty(result?.RefreshToken));
    }

    [Fact]
    public async Task RegisterAsync_UsernameTaken_ThrowsException()
    {
        // Arrange
        var existingUsers = new List<User>
        {
            new User
            {
                Username = "ExistingUser",
                Email = "existing@example.com",
                Password = "test",
                PasswordSalt = "test"
            }
        };

        var dto = new UserRegisterDTO(
            "ExistingUser",
            "new@example.com",
            "password123",
            "test",
            "password123",
            Gender.MALE,
            Role.USER
        );

        _userRepositoryMock
            .Setup(r =>
                r.GetAllAsync(
                    It.IsAny<List<Expression<Func<User, bool>>>>(),
                    It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>?>(),
                    It.IsAny<string?>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>()
                )
            )
            .ReturnsAsync(existingUsers);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterAsync(dto));
    }

    [Fact]
    public async Task LogoutAsync_ValidUser_ClearsTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            RefreshToken = "token",
            RefreshTokenExpiryTime = DateTime.UtcNow,
            Username = "test",
            Email = "test@gmail.com",
            Password = "test",
            PasswordSalt = "test"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _userService.LogoutAsync(userId);

        // Assert
        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiryTime);
        _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UserIsSelf_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "test",
            Email = "test@gmail.com",
            Password = "test",
            PasswordSalt = "test"
        };

        var claims = new List<Claim>
        {
            new Claim("UserId", userId.ToString()),
            new Claim(ClaimTypes.Role, Role.USER.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(h => h.HttpContext).Returns(httpContext);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result?.Username);
    }
}
