using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleverDocs.Core.Entities;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
            
        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
            
        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email");
            
        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("ix_users_created_at");
    }
} 