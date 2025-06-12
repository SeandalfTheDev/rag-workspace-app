using CleverDocs.Infrastructure.Data;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Testcontainers.PostgreSql;

namespace CleverDocs.Integration.Tests.Helpers;

public class WebApiTestFactory :WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
      .WithImage("pgvector/pgvector:pg17")
      .WithPortBinding(5432, true)
      .WithDatabase("CleverDocs")
      .WithUsername("postgres")
      .WithPassword("postgres")
      .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready -U postgres -d CleverDocs"))
      .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await EnsureDatabaseAsync();
    }

    private async Task EnsureDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseNpgsql(_dbContainer.GetConnectionString(), opt =>
        {
            opt.UseVector();
        })
        .UseSnakeCaseNamingConvention()
        .Options;
        var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var logger = new LoggerConfiguration()    
            .WriteTo.Console()
            .CreateLogger();
        
        Log.Logger = logger;

        builder.UseSerilog(logger);
        
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false);
        });

        builder.ConfigureServices(services =>
        {
            // Remove the existing database registration if it exists
            var descriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
            options.UseNpgsql(_dbContainer.GetConnectionString(), opt => opt.UseVector());
            options.UseSnakeCaseNamingConvention();
            });

            // Configure logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(logger, dispose: true);
            });

            services.AddSerilog(logger);
        });

        return base.CreateHost(builder);
    }
}