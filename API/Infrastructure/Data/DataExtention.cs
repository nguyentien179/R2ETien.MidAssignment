using System;
using Microsoft.EntityFrameworkCore;

namespace mid_assignment.Infrastructure.Data;

public static class DataExtention
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}
