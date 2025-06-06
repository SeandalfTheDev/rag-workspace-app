using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pgvector.EntityFrameworkCore;

namespace CleverDocs.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
  public ApplicationDbContext CreateDbContext(string[] args)
  {
          var environement = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CleverDocs.WebApi"))
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environement}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                              ?? configuration.GetConnectionString("Postgres")
                              ?? "Host=localhost;Database=cleverdocs;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            npgsqlOptions.UseVector();
        })
        .UseSnakeCaseNamingConvention();

        return new ApplicationDbContext(optionsBuilder.Options);
  }
}