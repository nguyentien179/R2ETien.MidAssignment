using System;
using Microsoft.AspNetCore.Mvc;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.BookReview;

namespace mid_assignment.Presentations.Endpoints;

public static class BookReviewEndpoints
{
    public static void MapBookReviewEndpoints(this IEndpointRouteBuilder group)
    {
        var bookReviewGroup = group.MapGroup("/bookReview").WithTags("Book Review");
        var authBookReviewGroup = bookReviewGroup.MapGroup("").RequireAuthorization();
        bookReviewGroup.MapGet("/withBook/{bookId:guid}", GetAllByBookIdAsync);
        bookReviewGroup.MapGet("/{id:guid}", GetByIdAsync);
        authBookReviewGroup.MapPost("/{bookId:guid}", CreateAsync);
        authBookReviewGroup.MapPut("/{id:guid}", UpdateAsync);
        authBookReviewGroup.MapDelete("/{id:guid}", DeleteAsync);
    }

    private static async Task<IResult> GetAllByBookIdAsync(
        [FromServices] IBookReviewService service,
        Guid bookId
    )
    {
        var reviews = await service.GetAllByBookIdAsync(bookId);
        return Results.Ok(reviews);
    }

    private static async Task<IResult> GetByIdAsync(
        [FromServices] IBookReviewService service,
        Guid id
    )
    {
        var review = await service.GetByIdAsync(id);
        return Results.Ok(review);
    }

    private static async Task<IResult> CreateAsync(
        [FromServices] IBookReviewService service,
        [FromBody] CreateBookReviewDTO dto,
        Guid bookId
    )
    {
        await service.CreateAsync(dto, bookId);
        return Results.Created("/bookReview", dto);
    }

    private static async Task<IResult> UpdateAsync(
        [FromServices] IBookReviewService service,
        [FromBody] UpdateBookReviewDTO dto,
        Guid id
    )
    {
        await service.UpdateAsync(id, dto);
        return Results.Ok("Updated");
    }

    private static async Task<IResult> DeleteAsync(
        [FromServices] IBookReviewService service,
        Guid id
    )
    {
        await service.DeleteAsync(id);
        return Results.NoContent();
    }
}
