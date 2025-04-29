using System;
using mid_assignment.Domain.Entities;
using mid_assignment.Presentations.DTO.User;

namespace mid_assignment.Mapping;

public static class UserMapping
{
    public static User ToEntity(this UserRegisterDTO dto, string hashedPassword, string salt)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            Password = hashedPassword,
            PasswordSalt = salt,
            Gender = dto.Gender,
            Role = dto.Role,
        };
    }

    public static UserDTO ToDTO(this User user)
    {
        return new UserDTO(user.Id, user.Username, user.Email, user.Gender, user.Role);
    }
}
