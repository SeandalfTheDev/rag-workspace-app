using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleverDocs.Core.Entities;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("workspace_members");
        
        // Composite primary key (WorkspaceId + UserId)
        builder.HasKey(wm => new { wm.WorkspaceId, wm.UserId });
        
        builder.Property(wm => wm.WorkspaceId)
            .HasColumnName("workspace_id")
            .IsRequired();
            
        builder.Property(wm => wm.UserId)
            .HasColumnName("user_id")
            .IsRequired();
            
        builder.Property(wm => wm.Role)
            .HasColumnName("role")
            .HasConversion<int>()
            .IsRequired();
            
        builder.Property(wm => wm.JoinedAt)
            .HasColumnName("joined_at")
            .IsRequired();
            
        builder.Property(wm => wm.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
            
        // Indexes
        builder.HasIndex(wm => wm.WorkspaceId)
            .HasDatabaseName("ix_workspace_members_workspace_id");
            
        builder.HasIndex(wm => wm.UserId)
            .HasDatabaseName("ix_workspace_members_user_id");
            
        builder.HasIndex(wm => wm.Role)
            .HasDatabaseName("ix_workspace_members_role");
            
        builder.HasIndex(wm => wm.JoinedAt)
            .HasDatabaseName("ix_workspace_members_joined_at");
            
        // Relationships
        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(wm => wm.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(wm => wm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 