using System;
using FluentValidation;
using mid_assignment.Application.Common.Constants;

namespace mid_assignment.Presentations.DTO.Category;

public class CreateCategoryDTOValidator : AbstractValidator<CreateCategoryDTO>
{
    public CreateCategoryDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ErrorMessages.Required)
            .MaximumLength(40)
            .WithMessage("No more than 40 characters");
    }
}
