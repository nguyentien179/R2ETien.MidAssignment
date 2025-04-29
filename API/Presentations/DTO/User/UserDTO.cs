using mid_assignment.Domain.Enum;

namespace mid_assignment.Presentations.DTO.User;

public record class UserDTO(Guid Id, string Username, string Email, Gender Gender, Role Role);
