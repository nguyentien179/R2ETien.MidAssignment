using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mid_assignment.Application;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Middleware;
using mid_assignment.Presentations.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration;

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationServices();

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(
        "PerUserMonthlyLimit",
        context =>
        {
            var userId =
                context.User.FindFirstValue("UserId") ?? context.Request.Query["UserId"].ToString();

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: userId,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3, // 3 requests
                    Window = TimeSpan.FromDays(30),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                }
            );
        }
    );

    // Add custom response when limit is exceeded
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync(
            "You've exceeded the maximum of 3 borrowing requests per month. Please try again later.",
            token
        );
    };
});

//Only read access token from cookie
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["JwtSettings:SecretKey"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet(
        "/",
        context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        }
    );
}

app.UseRateLimiter();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapApiEndpoints();

app.MigrateDb();

app.Run();
