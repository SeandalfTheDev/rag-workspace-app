using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleverDocs.Core.Entities;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
            
        builder.Property(d => d.WorkspaceId)
            .HasColumnName("workspace_id")
            .IsRequired();
            
        builder.Property(d => d.ObjectId)
            .HasColumnName("object_id")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(d => d.FileName)
            .HasColumnName("filename")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(d => d.OriginalFileName)
            .HasColumnName("original_filename")
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(d => d.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(d => d.FileSize)
            .HasColumnName("file_size")
            .IsRequired();
            
        builder.Property(d => d.FilePath)
            .HasColumnName("file_path")
            .HasMaxLength(500)
            .IsRequired();
            
        builder.Property(d => d.FileExtension)
            .HasColumnName("file_extension")
            .HasMaxLength(10)
            .IsRequired();
            
        builder.Property(d => d.UploadStatus)
            .HasColumnName("upload_status")
            .HasConversion<int>()
            .IsRequired();
            
        builder.Property(d => d.UploadedBy)
            .HasColumnName("uploaded_by")
            .IsRequired();
            
        builder.Property(d => d.UploadedAt)
            .HasColumnName("uploaded_at")
            .IsRequired();
            
        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
            
        // Indexes
        builder.HasIndex(d => d.WorkspaceId)
            .HasDatabaseName("ix_documents_workspace_id");
            
        builder.HasIndex(d => d.UploadedBy)
            .HasDatabaseName("ix_documents_uploaded_by");
            
        builder.HasIndex(d => d.UploadStatus)
            .HasDatabaseName("ix_documents_upload_status");
            
        builder.HasIndex(d => d.CreatedAt)
            .HasDatabaseName("ix_documents_created_at");
            
        builder.HasIndex(d => d.ObjectId)
            .IsUnique()
            .HasDatabaseName("ix_documents_object_id");
    }
} 