using CleverDocs.Domain.Auth;
using CleverDocs.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
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
    
    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        RoleManager<IdentityRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            if (!await roleManager.RoleExistsAsync(Roles.AppAdmin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.AppAdmin));
            }
            if (!await roleManager.RoleExistsAsync(Roles.AppUser))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.AppUser));
            }

            app.Logger.LogInformation("Successfully created roles.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while seeding initial data.");
            throw;
        }
    }
}