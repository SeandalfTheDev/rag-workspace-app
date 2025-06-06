using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleverDocs.Core.Entities;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("workspaces");
        
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
            
        builder.Property(w => w.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(w => w.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);
            
        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(w => w.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
            
        // Indexes
        builder.HasIndex(w => w.Name)
            .HasDatabaseName("ix_workspaces_name");
            
        builder.HasIndex(w => w.CreatedAt)
            .HasDatabaseName("ix_workspaces_created_at");
    }
} 