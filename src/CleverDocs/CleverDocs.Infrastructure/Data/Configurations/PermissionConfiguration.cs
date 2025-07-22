using CleverDocs.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("ix_permissions_name_unique");

        // Timestamp defaults
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())")
            .ValueGeneratedOnUpdate();
    }
}
