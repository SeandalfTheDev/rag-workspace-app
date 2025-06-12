using System.Reflection;
using CleverDocs.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleverDocs.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
  public DbSet<User> Users { get; set;}
  public DbSet<Workspace> Workspaces { get; set;}
  public DbSet<WorkspaceMember> WorkspaceMembers { get; set;}
  public DbSet<Document> Documents { get; set;}
  public DbSet<DocumentChunk> DocumentChunks { get; set;}

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    modelBuilder.HasPostgresExtension("vector");
    base.OnModelCreating(modelBuilder);
  }
}