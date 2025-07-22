using CleverDocs.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        // Composite Primary Key with Scope
        builder.HasKey(ur => new { ur.UserId, ur.RoleId, ur.ScopeId, ur.ScopeType });

        // Properties
        builder.Property(ur => ur.ScopeType)
            .HasMaxLength(50);

        // Timestamp defaults
        builder.Property(ur => ur.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        // Nullable expiration
        builder.Property(ur => ur.ExpiresAt)
            .IsRequired(false);

        // Indexes for common queries
        builder.HasIndex(ur => ur.UserId).HasDatabaseName("ix_user_roles_user_id");
        builder.HasIndex(ur => ur.RoleId).HasDatabaseName("ix_user_roles_role_id");;
        builder.HasIndex(ur => new { ur.ScopeId, ur.ScopeType }).HasDatabaseName("ix_scope_id_scope_type");
        builder.HasIndex(ur => ur.ExpiresAt).HasDatabaseName("ix_user_roles_expires_at");

        // Relationships
        builder.HasOne<User>()
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
