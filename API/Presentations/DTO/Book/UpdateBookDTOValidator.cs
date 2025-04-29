using System;
using FluentValidation;
using mid_assignment.Application.Common.Constants;

namespace mid_assignment.Presentations.DTO.Book;

public class UpdateBookDTOValidator : AbstractValidator<UpdateBookDTO>
{
    public UpdateBookDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessages.Required)
            .MaximumLength(40)
            .WithMessage("No more than 40 characters");
        RuleFor(x => x.Author)
            .NotEmpty()
            .WithMessage(ErrorMessages.Required)
            .MaximumLength(50)
            .WithMessage("No more than 50 characters");
        RuleFor(x => x.CategoryId).NotEqual(Guid.Empty).WithMessage(ErrorMessages.InvalidGuid);
    }
}
