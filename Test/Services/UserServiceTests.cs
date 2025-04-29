using System.Security.Cryptography;
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
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _userService = new UserService(_userRepositoryMock.Object, _configurationMock.Object);
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
    public async Task LoginAsync_ValidCredentials_ReturnsUserAndToken()
    {
        // Arrange
        string usernameOrEmail = "testuser";
        string password = "password";
        string salt = Convert.ToBase64String(new byte[16]);
        string hashedPassword = HashPassword(password, salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = usernameOrEmail,
            Email = "test@example.com",
            Password = hashedPassword,
            PasswordSalt = salt,
            Role = Role.USER,
        };

        var dto = new UserLoginDTO(usernameOrEmail, password);

        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsernameOrEmailAsync(usernameOrEmail))
            .ReturnsAsync(user);
        _configurationMock
            .Setup(c => c["JwtSettings:SecretKey"])
            .Returns("12951111111116714340772801558047");

        // Act
        var result = await _userService.LoginAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Value.User.Id);
        Assert.NotEmpty(result.Value.Token);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException_UserNotFound()
    {
        // Arrange
        string usernameOrEmail = "nonexistentuser";
        string password = "password";

        var dto = new UserLoginDTO(usernameOrEmail, password);

        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsernameOrEmailAsync(usernameOrEmail))
            .ReturnsAsync((User?)null); // Simulate user not found

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException_InvalidPassword()
    {
        // Arrange
        string usernameOrEmail = "testuser";
        string inputPassword = "wrongpassword";
        string correctPassword = "correctpassword";
        string salt = Convert.ToBase64String(new byte[16]);
        string correctHashedPassword = HashPassword(correctPassword, salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = usernameOrEmail,
            Email = "test@example.com",
            Password = correctHashedPassword,
            PasswordSalt = salt,
            Role = Role.USER,
        };

        var dto = new UserLoginDTO(usernameOrEmail, inputPassword);

        _userRepositoryMock
            .Setup(repo => repo.GetUserByUsernameOrEmailAsync(usernameOrEmail))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(dto));
    }

    [Fact]
    public async Task RegisterAsync_ValidInput_RegistersUser()
    {
        string salt = Convert.ToBase64String(new byte[16]);
        var dto = new UserRegisterDTO(
            "newuser",
            "newuser@example.com",
            "password",
            salt,
            "password",
            Gender.Male,
            Role.USER
        );

        var hashedPassword = HashPassword(dto.Password, salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            Role = dto.Role,
            Password = hashedPassword,
            PasswordSalt = salt,
        };

        _userRepositoryMock
            .Setup(repo => repo.RegisterAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.RegisterAsync(dto);

        // Assert
        _userRepositoryMock.Verify(
            repo =>
                repo.RegisterAsync(
                    It.Is<User>(u =>
                        u.Username == dto.Username
                        && u.Email == dto.Email
                        && u.Password != null // Ensure hashed password
                        && u.PasswordSalt != null // Ensure password salt
                    )
                ),
            Times.Once
        );
    }
}
