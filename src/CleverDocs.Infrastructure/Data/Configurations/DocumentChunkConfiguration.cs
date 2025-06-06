using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleverDocs.Core.Entities;
using CleverDocs.Infrastructure.Data.Converters;
using Pgvector.EntityFrameworkCore;

namespace CleverDocs.Infrastructure.Data.Configurations;

public class DocumentChunkConfiguration : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable("document_chunks");
        
        // Composite primary key (DocumentId + ChunkIndex)
        builder.HasKey(dc => new { dc.DocumentId, dc.ChunkIndex });
        
        builder.Property(dc => dc.DocumentId)
            .HasColumnName("document_id")
            .IsRequired();
            
        builder.Property(dc => dc.ChunkIndex)
            .HasColumnName("chunk_index")
            .IsRequired();
            
        builder.Property(dc => dc.Content)
            .HasColumnName("content")
            .HasColumnType("text")
            .IsRequired();
            
        builder.Property(dc => dc.Embedding)
            .HasColumnName("embedding")
            .HasColumnType("vector(768)")
            .HasConversion<VectorConverter>()
            .IsRequired();
            
        builder.Property(dc => dc.Metadata)
            .HasColumnName("metadata")
            .HasColumnType("jsonb")
            .HasConversion<DictionaryJsonConverter>()
            .IsRequired();
            
        builder.Property(dc => dc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        // Indexes
        builder.HasIndex(dc => dc.DocumentId)
            .HasDatabaseName("ix_document_chunks_document_id");
            
        builder.HasIndex(dc => dc.CreatedAt)
            .HasDatabaseName("ix_document_chunks_created_at");
            
        // Vector similarity index for pgvector
        builder.HasIndex(dc => dc.Embedding)
            .HasDatabaseName("ix_document_chunks_embedding_cosine");
            
        // Relationships
        builder.HasOne<Document>()
            .WithMany()
            .HasForeignKey(dc => dc.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 