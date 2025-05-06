using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CloudinaryDotNet;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using mid_assignment.Application;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Infrastructure.EntityConfig;
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

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(
        "PerUserMonthlyLimit",
        httpContext =>
        {
            var userId =
                httpContext.User.FindFirstValue("UserId")
                ?? httpContext.Request.Query["UserId"].ToString();

            if (string.IsNullOrEmpty(userId))
            {
                userId = "anonymous";
            }

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: userId,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromDays(30),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0, // Optional: no queuing
                    AutoReplenishment = true // Ensure permits refill every window
                }
            );
        }
    );

    options.RejectionStatusCode = 429;

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "text/plain";
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
            ValidateAudience = false,
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
            },
        };
    });

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<IgnoreAntiforgeryTokenAttribute>();
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
});

builder.Services.AddAuthorization();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
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
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.UseCors();

app.UseRateLimiter();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapApiEndpoints();

app.MigrateDb();

app.Run();
