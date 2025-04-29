using System;
using FluentValidation.Results;

namespace mid_assignment.Application.Helpers.Validation;

public static class ValidationExtensions
{
    public static IResult ToValidationError(this ValidationResult result)
    {
        var errors = result.Errors.Select(e => new
        {
            Field = e.PropertyName,
            Error = e.ErrorMessage,
        });

        return Results.UnprocessableEntity(
            new
            {
                Code = "VALIDATION_FAILED",
                Message = "Request validation failed.",
                Errors = errors,
            }
        );
    }
}
