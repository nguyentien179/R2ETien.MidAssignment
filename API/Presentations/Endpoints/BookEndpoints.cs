using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mid_assignment.Application.Common.Filter;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.Book;

namespace mid_assignment.Presentations.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder group)
    {
        var bookGroup = group.MapGroup("/books").WithTags("Books");
        var adminBookGroup = bookGroup
            .MapGroup("")
            .DisableAntiforgery()
            .RequireAuthorization(policy => policy.RequireRole(Role.ADMIN.ToString()));
        bookGroup.MapGet("/", GetAllAsync);
        bookGroup.MapGet("/{id:guid}", GetByIdAsync);
        adminBookGroup
            .MapPost("/", CreateAsync)
            .AddEndpointFilter<ValidationFilter<CreateBookDTO>>()
            .WithMetadata(new IgnoreAntiforgeryTokenAttribute());
        adminBookGroup
            .MapPut("/{id:guid}", UpdateAsync)
            .AddEndpointFilter<ValidationFilter<UpdateBookDTO>>()
            .WithMetadata(new IgnoreAntiforgeryTokenAttribute());
        adminBookGroup.MapDelete("/{id:guid}", DeleteAsync);
    }

    private static async Task<IResult> GetAllAsync(
        [FromServices] IBookService service,
        [FromQuery] string? nameFilter,
        [FromQuery] string? authorFilter,
        [FromQuery] Guid? categoryId,
        [FromQuery] SortDirection? sortOrder,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize
    )
    {
        var filters = new List<Expression<Func<Book, bool>>>();

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            filters.Add(b => EF.Functions.Like(b.Name, $"%{nameFilter}%"));
        }

        if (!string.IsNullOrWhiteSpace(authorFilter))
        {
            filters.Add(b => EF.Functions.Like(b.Author, $"%{authorFilter}%"));
        }

        if (categoryId.HasValue)
        {
            filters.Add(b => b.CategoryId == categoryId.Value);
        }

        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = sortOrder switch
        {
            SortDirection.ASCENDING => q => q.OrderBy(b => b.Name),
            SortDirection.DESCENDING => q => q.OrderByDescending(b => b.Name),
            _ => null,
        };

        var result = await service.GetAllAsync(
            filters,
            orderBy,
            includeProperties: "Category",
            pageNumber,
            pageSize
        );

        return Results.Ok(result);
    }

    private static async Task<IResult> GetByIdAsync(
        [FromServices] IBookService bookService,
        Guid id
    )
    {
        var book = await bookService.GetByIdAsync(id);
        return Results.Ok(book);
    }

    private static async Task<IResult> CreateAsync(
        [FromForm] CreateBookInputDTO dto,
        [FromServices] IBookService bookService
    )
    {
        await bookService.CreateAsync(dto);
        return Results.Created("/books", dto);
    }

    private static async Task<IResult> UpdateAsync(
        [FromForm] UpdateBookDTO dto,
        [FromServices] IBookService bookService,
        Guid id
    )
    {
        await bookService.UpdateAsync(dto, id);
        return Results.Ok("updated");
    }

    private static async Task<IResult> DeleteAsync([FromServices] IBookService bookService, Guid id)
    {
        await bookService.DeleteAsync(id);
        return Results.NoContent();
    }
}
