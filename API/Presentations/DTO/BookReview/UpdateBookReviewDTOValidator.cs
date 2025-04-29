using System;
using FluentValidation;

namespace mid_assignment.Presentations.DTO.BookReview;

public class UpdateBookReviewDTOValidator : AbstractValidator<UpdateBookReviewDTO>
{
    public UpdateBookReviewDTOValidator()
    {
        RuleFor(x => x.Rating)
            .NotEmpty()
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5 stars");
        ;
        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Comment))
            .WithMessage("Comment cannot exceed 1000 characters");
    }
}
