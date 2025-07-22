using CleverDocs.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        // Primary Key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.IsSystemRole)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("ix_roles_name_unique");

        // Timestamp defaults
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(r => r.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnUpdate();

        // Seed data for system roles
        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "SuperAdmin",
                Description = "Has full access to all features and settings",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = 2,
                Name = "WorkspaceOwner",
                Description = "Has full access to a specific workspace",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = 3,
                Name = "WorkspaceAdmin",
                Description = "Can manage most workspace settings and members",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = 4,
                Name = "Member",
                Description = "Basic access to workspace features",
                IsSystemRole = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
