using System;
using System.Linq.Expressions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using mid_assignment.Application.Common.Filter;
using mid_assignment.Application.Interfaces;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Presentations.DTO.User;

namespace mid_assignment.Presentations.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder group)
    {
        var userGroup = group.MapGroup("/users").WithTags("Users");
        var adminUserGroup = userGroup
            .MapGroup("")
            .RequireAuthorization(policy => policy.RequireRole(Role.ADMIN.ToString()));
        adminUserGroup.MapGet("/", GetAllAsync);
        adminUserGroup.MapDelete("/{id:guid}", DeleteAsync);

        userGroup.MapGet("/{id:guid}", GetByIdAsync).RequireAuthorization();
        userGroup.MapGet("/current-user", GetCurrentUserAsync).RequireAuthorization();
        userGroup
            .MapPost("/register", RegisterAsync)
            .AddEndpointFilter<ValidationFilter<UserRegisterDTO>>();
        userGroup.MapPost("/login", LoginAsync).AddEndpointFilter<ValidationFilter<UserLoginDTO>>();
        userGroup.MapPost("/logout", LogoutAsync).RequireAuthorization();
        userGroup.MapPost("/refresh-token", RefreshTokenAsync);
    }

    private static async Task<IResult> RefreshTokenAsync(
        IUserService service,
        HttpContext httpContext
    )
    {
        var refreshToken = httpContext.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Results.Unauthorized();
        }

        try
        {
            // Validate the refresh token against database
            var userId = await service.GetByRefreshTokenAsync(refreshToken);
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var tokenResponse = await service.RefreshTokenAsync(refreshToken);
            httpContext.Response.Cookies.Append(
                "AccessToken",
                tokenResponse!.AccessToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(60),
                }
            );
            httpContext.Response.Cookies.Append(
                "RefreshToken",
                tokenResponse.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                }
            );

            return Results.Ok(new { Message = "Tokens refreshed successfully" });
        }
        catch (Exception)
        {
            httpContext.Response.Cookies.Delete("AccessToken");
            httpContext.Response.Cookies.Delete("RefreshToken");
            return Results.Unauthorized();
        }
    }

    private static async Task<IResult> DeleteAsync([FromServices] IUserService service, Guid id)
    {
        var userId = service.GetCurrentUserId();
        if (userId == null || userId.Value == id)
        {
            return Results.Unauthorized();
        }
        await service.DeleteAsync(id);
        return Results.Ok("User deleted successfully.");
    }

    private static async Task<IResult> GetCurrentUserAsync([FromServices] IUserService service)
    {
        var user = await service.GetCurrentUserAsync();
        return Results.Ok(
            new
            {
                user.Id,
                user.Username,
                user.Email,
                user.Role
            }
        );
    }

    private static async Task<IResult> LogoutAsync(
        [FromServices] IUserService service,
        HttpContext httpContext
    )
    {
        var userId = service.GetCurrentUserId();
        if (userId == null)
        {
            return Results.Unauthorized();
        }
        httpContext.Response.Cookies.Delete("AccessToken");
        httpContext.Response.Cookies.Delete("RefreshToken");
        await service.LogoutAsync(userId.Value);
        return Results.Ok("Logout successful.");
    }

    private static async Task<IResult> GetAllAsync(
        [FromServices] IUserService service,
        [FromQuery] string? emailFilter,
        [FromQuery] string? usernameFilter,
        [FromQuery] SortDirection? sortOrder,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize
    )
    {
        var filters = new List<Expression<Func<User, bool>>>();

        if (!string.IsNullOrEmpty(emailFilter))
        {
            filters.Add(u => u.Email == emailFilter);
        }

        if (!string.IsNullOrEmpty(usernameFilter))
        {
            filters.Add(u => u.Username == usernameFilter);
        }
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null;

        if (sortOrder.HasValue)
        {
            orderBy =
                sortOrder == SortDirection.ASCENDING
                    ? query => query.OrderBy(u => u.Id)
                    : query => query.OrderByDescending(u => u.Id);
        }

        var result = await service.GetAllAsync(filters, orderBy, null, pageNumber, pageSize);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetByIdAsync([FromServices] IUserService service, Guid id)
    {
        var user = await service.GetByIdAsync(id);
        return Results.Ok(user);
    }

    private static async Task<IResult> RegisterAsync(
        [FromBody] UserRegisterDTO dto,
        [FromServices] IUserService service
    )
    {
        await service.RegisterAsync(dto);
        return Results.Created("/api/employees", dto);
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] UserLoginDTO dto,
        [FromServices] IUserService service,
        HttpContext httpContext
    )
    {
        var result = await service.LoginAsync(dto);
        var (user, token, refreshToken) = result.Value;
        httpContext.Response.Cookies.Append(
            "AccessToken",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(60),
            }
        );
        httpContext.Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            }
        );

        return Results.Ok(
            new
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                RefreshToken = refreshToken,
            }
        );
    }
}
