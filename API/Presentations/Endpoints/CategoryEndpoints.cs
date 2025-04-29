using System;
using Microsoft.AspNetCore.Mvc;
using mid_assignment.Application.Common.Filter;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.Category;

namespace mid_assignment.Presentations.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder group)
    {
        var categoryGroup = group.MapGroup("/category").WithTags("Category");
        var adminCategoryGroup = categoryGroup
            .MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole(Role.ADMIN.ToString()));
        categoryGroup.MapGet("/", GetAllAsync);
        categoryGroup.MapGet("/{id:guid}", GetByIdAsync);
        adminCategoryGroup
            .MapPost("/", CreateAsync)
            .AddEndpointFilter<ValidationFilter<CreateCategoryDTO>>();
        adminCategoryGroup
            .MapPut("/{id:guid}", UpdateASync)
            .AddEndpointFilter<ValidationFilter<UpdateCategoryDTO>>();
        adminCategoryGroup.MapDelete("/{id:guid}", DeleteAsync);
    }

    private static async Task<IResult> GetAllAsync([FromServices] ICategoryService categoryService)
    {
        var categories = await categoryService.GetAllAsync();
        return Results.Ok(categories);
    }

    private static async Task<IResult> GetByIdAsync(
        [FromServices] ICategoryService categoryService,
        Guid id
    )
    {
        var category = await categoryService.GetByIdAsync(id);
        return Results.Ok(category);
    }

    private static async Task<IResult> CreateAsync(
        [FromServices] ICategoryService categoryService,
        [FromBody] CreateCategoryDTO dto
    )
    {
        await categoryService.CreateAsync(dto);
        return Results.Created("/category ", dto);
    }

    private static async Task<IResult> UpdateASync(
        [FromServices] ICategoryService categoryService,
        [FromBody] UpdateCategoryDTO dto,
        Guid id
    )
    {
        await categoryService.UpdateAsync(dto, id);
        return Results.Ok("updated");
    }

    private static async Task<IResult> DeleteAsync(
        [FromServices] ICategoryService categoryService,
        Guid id
    )
    {
        await categoryService.DeleteAsync(id);
        return Results.NoContent();
    }
}
