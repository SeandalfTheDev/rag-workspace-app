using CleverDocs.Domain.Auth;
using CleverDocs.Infrastructure.Data.Contexts;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace CleverDocs.IntegrationTests.Infrastructure;

public class CleverDocsWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("pgvector/pgvector:pg17")
        .WithPortBinding(5432, true)
        .WithDatabase("CleverDocs")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready -U postgres -d CleverDocs"))
        .Build();
    
    public string ConnectionString => _dbContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ASPNETCORE_ENVIRONMENT", "Testing");
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<AppIdentityDbContext>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
            
            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
            
            try
            {
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var identityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var roleManager =
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                
                appDbContext.Database.Migrate();
                identityDbContext.Database.Migrate();
                
                if (!roleManager.RoleExistsAsync(Roles.AppAdmin).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(Roles.AppAdmin));
                }
                if (!roleManager.RoleExistsAsync(Roles.AppUser).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(Roles.AppUser));
                }
            }
            catch
            {
                throw;
            }
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}