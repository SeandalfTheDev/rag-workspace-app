using CleverDocs.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CleverDocs.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using AppDbContext applicationDbContext =
            scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await using AppIdentityDbContext identityDbContext =
            scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

        try
        {
            await applicationDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Application database migrations applied successfully.");

            await identityDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Identity database migrations applied successfully.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "An error occurred while applying database migrations.");
            throw;
        }
    }
}