using System;
using FluentValidation;
using mid_assignment.Application.Common.Constants;

namespace mid_assignment.Presentations.DTO.User;

public class UserLoginDTOValidator : AbstractValidator<UserLoginDTO>
{
    public UserLoginDTOValidator()
    {
        RuleFor(x => x.UsernameOrEmail).NotEmpty().WithMessage(ErrorMessages.Required);
        RuleFor(x => x.UsernameOrEmail).MaximumLength(30).WithMessage("No more than 30 characters");
        RuleFor(x => x.Password).NotEmpty().WithMessage(ErrorMessages.Required);
    }
}
