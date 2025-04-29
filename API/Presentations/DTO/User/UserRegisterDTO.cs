using mid_assignment.Domain.Enum;

namespace mid_assignment.Presentations.DTO.User;

public record class UserRegisterDTO(
    string Username,
    string Email,
    string Password,
    string PasswordSalt,
    string ConfirmPassword,
    Gender Gender,
    Role Role
);
