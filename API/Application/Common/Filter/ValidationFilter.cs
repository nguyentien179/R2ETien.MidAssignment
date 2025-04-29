using System;
using FluentValidation;

namespace mid_assignment.Application.Common.Filter;

public class ValidationFilter<T> : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var dto = context.Arguments.OfType<T>().FirstOrDefault();
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (dto is null || validator is null)
        {
            return await next(context);
        }

        var results = await validator.ValidateAsync(dto);

        if (!results.IsValid)
        {
            var errors = results.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            return Results.BadRequest(errors);
        }

        return await next(context);
    }
}
