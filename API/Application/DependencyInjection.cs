using System;
using mid_assignment.Application.Interfaces;
using mid_assignment.Application.Services;
using mid_assignment.Infrastructure.Helper;
using mid_assignment.Infrastructure.Repositories;
using mid_assignment.Infrastructure.Repositories.Interfaces;

namespace mid_assignment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(GeneralRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookService, BookService>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IBookBorrowingRequestRepository, BookBorrowingRequestRepository>();
        services.AddScoped<IBookBorrowingRequestService, BookBorrowingRequestService>();

        services.AddScoped<IBookReviewRepository, BookReviewRepository>();
        services.AddScoped<IBookReviewService, BookReviewService>();

        services.AddScoped<IImageUploader, CloudinaryImageUploader>();

        return services;
    }
}
