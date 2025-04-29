using System;
using System.Text.RegularExpressions;
using FluentValidation;
using mid_assignment.Application.Common.Constants;

namespace mid_assignment.Presentations.DTO.User;

public class UserRegisterDTOValidator : AbstractValidator<UserRegisterDTO>
{
    public UserRegisterDTOValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(ErrorMessages.Required)
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ErrorMessages.Required)
            .EmailAddress()
            .WithMessage(ErrorMessages.EmailInvalid);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ErrorMessages.Required)
            .MinimumLength(8)
            .WithMessage(ErrorMessages.PasswordTooShort)
            .Must(ContainUppercase)
            .WithMessage(ErrorMessages.PasswordTooWeak)
            .Must(ContainDigit)
            .WithMessage(ErrorMessages.PasswordTooWeak)
            .Must(ContainSymbol)
            .WithMessage(ErrorMessages.PasswordTooWeak);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");

        RuleFor(x => x.Gender).IsInEnum().WithMessage("Gender must be a valid enum value.");

        RuleFor(x => x.Role).IsInEnum().WithMessage("Role must be a valid enum value.");
    }

    private bool ContainUppercase(string password) => password.Any(char.IsUpper);

    private bool ContainDigit(string password) => password.Any(char.IsDigit);

    private bool ContainSymbol(string password) =>
        Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>_\-+=~`\\/\[\];:]");
}
