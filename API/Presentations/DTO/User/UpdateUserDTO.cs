using mid_assignment.Domain.Enum;

namespace mid_assignment.Presentations.DTO.User;

public record class UpdateUserDTO(string Username, string Email, Gender Gender, Role Role);
