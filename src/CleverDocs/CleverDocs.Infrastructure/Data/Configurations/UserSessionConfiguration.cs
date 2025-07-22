using CleverDocs.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        // Primary Key
        builder.HasKey(us => us.Id);

        // Properties
        builder.Property(us => us.RefreshToken)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(us => us.IpAddress)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(us => us.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(us => us.Revoked)
            .IsRequired()
            .HasDefaultValue(false);

        // Timestamps
        builder.Property(us => us.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(us => us.ExpiresAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(us => us.UserId).HasDatabaseName("ix_user_session_user_id");
        builder.HasIndex(us => us.RefreshToken)
            .IsUnique()
            .HasDatabaseName("ix_user_session_refresh_token_unique");
        builder.HasIndex(us => us.ExpiresAt).HasDatabaseName("ix_user_session_expires_at");

        // Relationships
        builder.HasOne(us => us.User)
            .WithMany(u => u.UserSessions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
