using CleverDocs.Domain.Auth;
using Microsoft.EntityFrameworkCore;

namespace CleverDocs.Infrastructure.Data.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Application);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    
    public DbSet<User> Users { get; set; } = null!;
}