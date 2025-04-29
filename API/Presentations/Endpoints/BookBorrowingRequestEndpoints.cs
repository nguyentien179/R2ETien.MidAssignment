using System;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Common.Models;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.BookBorrowingRequest;

namespace mid_assignment.Presentations.Endpoints;

public static class BookBorrowingRequestEndpoints
{
    public static void MapBookBorrowingRequestEndpoints(this IEndpointRouteBuilder group)
    {
        var BRgroup = group
            .MapGroup("/requests")
            .RequireAuthorization()
            .WithTags("Book Borrowing Requests");
        var AdminBRgroup = BRgroup
            .MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole(Role.ADMIN.ToString()));

        BRgroup.MapGet("/", GetAllAsync);
        BRgroup.MapGet("/{id:guid}", GetByIdAsync);
        BRgroup.MapGet("/getByUser/{userid:guid}", GetByUserIdAsync);
        BRgroup.MapPost("/", CreateAsync).RequireRateLimiting("PerUserMonthlyLimit");
        BRgroup.MapPut("/extend/{id:guid}", ExtendDueDate);
        AdminBRgroup.MapPut("/approve/{id:guid}", ApproveRequest);
        AdminBRgroup.MapPut("/reject/{id:guid}", RejectRequest);
        BRgroup.MapDelete("/{id:guid}", DeleteAsync);
    }

    private static async Task<IResult> GetByUserIdAsync(
        Guid userId,
        [FromServices] IBookBorrowingRequestService service
    )
    {
        var requests = await service.GetByUserIdAsync(userId);
        return Results.Ok(requests);
    }

    private static async Task<IResult> RejectRequest(
        Guid id,
        [FromServices] IBookBorrowingRequestService service
    )
    {
        await service.UpdateRequestStatusAsync(id, RequestStatus.REJECTED);
        return Results.Ok("Request successfully rejected.");
    }

    private static async Task<IResult> ApproveRequest(
        Guid id,
        [FromServices] IBookBorrowingRequestService service
    )
    {
        await service.UpdateRequestStatusAsync(id, RequestStatus.APPROVED);
        return Results.Ok("Request successfully approved.");
    }

    private static async Task<IResult> ExtendDueDate(
        Guid id,
        [FromServices] IBookBorrowingRequestService service
    )
    {
        await service.ExtendDueDate(id);
        return Results.Ok("Due Date entended");
    }

    private static async Task<IResult> GetAllAsync(
        [FromServices] IBookBorrowingRequestService service,
        [FromQuery] string? statusFilter,
        [FromQuery] SortDirection? sortOrder,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize
    )
    {
        var filters = new List<Expression<Func<BookBorrowingRequest, bool>>>();

        if (
            !string.IsNullOrEmpty(statusFilter)
            && Enum.TryParse<RequestStatus>(statusFilter, true, out var status)
        )
        {
            filters.Add(br => br.RequestStatus == status);
        }

        Func<IQueryable<BookBorrowingRequest>, IOrderedQueryable<BookBorrowingRequest>>? orderBy =
            null;

        if (sortOrder.HasValue)
        {
            orderBy =
                sortOrder == SortDirection.ASCENDING
                    ? query => query.OrderBy(br => br.RequestedDate)
                    : query => query.OrderByDescending(br => br.RequestedDate);
        }

        var result = await service.GetAllAsync(filters, orderBy, null, pageNumber, pageSize);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        [FromServices] IBookBorrowingRequestService service
    )
    {
        var request = await service.GetByIdAsync(id);
        return request is null ? Results.NotFound() : Results.Ok(request);
    }

    private static async Task<IResult> CreateAsync(
        [FromBody] CreateBookBorrowingRequestDTO dto,
        [FromServices] IBookBorrowingRequestService service
    )
    {
        await service.CreateAsync(dto);
        return Results.Created($"/requests", dto);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        [FromServices] IBookBorrowingRequestService service
    )
    {
        await service.DeleteAsync(id);
        return Results.NoContent();
    }
}
