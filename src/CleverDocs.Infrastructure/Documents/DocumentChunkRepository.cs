using CleverDocs.Core.Abstractions.Documents;
using CleverDocs.Core.Entities;
using CleverDocs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;

namespace CleverDocs.Infrastructure.Documents;

public class DocumentChunkRepository(ApplicationDbContext context) : IDocumentChunkRepository
{
    public async Task<List<DocumentChunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken token = default)
    {
        return await context.DocumentChunks
            .Where(chunk => chunk.DocumentId == documentId)
            .ToListAsync(token);
    }

    public async Task<List<DocumentChunk>> GetSimilarAsync(float[] embedding, CancellationToken token = default)
    {
        var similarityThreshold = 5.0;
        var similarDocuments = await context.DocumentChunks
            .OrderBy(chunk => chunk.Embedding.CosineDistance(embedding) < similarityThreshold)
            .Take(6)
            .ToListAsync(token);
        
        return similarDocuments;
    }

    public async Task<DocumentChunk?> GetChunkAsync(Guid documentId, int index, CancellationToken token = default)
    {
        return await context.DocumentChunks
            .SingleOrDefaultAsync(chunk => chunk.DocumentId == documentId, token);
    }

    public async Task<DocumentChunk?> AddAsync(DocumentChunk documentChunk, CancellationToken token = default)
    {
        await context.AddRangeAsync(documentChunk, token);
        await context.SaveChangesAsync(token);
        return documentChunk;
    }

    public async Task AddBatchAsync(List<DocumentChunk> documentChunks, CancellationToken token = default)
    {
        await context.AddRangeAsync(documentChunks, token);
        await context.SaveChangesAsync(token);
    }

    public async Task<DocumentChunk?> UpdateAsync(DocumentChunk documentChunk, CancellationToken token = default)
    {
        context.DocumentChunks.Update(documentChunk);
        await context.SaveChangesAsync(token);
        return documentChunk;
    }

    public async Task DeleteChunkAsync(Guid documentId, int index, CancellationToken token = default)
    {
        await context.DocumentChunks
            .Where(chunk => chunk.DocumentId == documentId && chunk.ChunkIndex == index)
            .ExecuteDeleteAsync(token);
    }

    public async Task DeleteAllChunksForDocumentAsync(Guid documentId, CancellationToken token = default)
    {
        await context.DocumentChunks
            .Where(chunk => chunk.DocumentId == documentId)
            .ExecuteDeleteAsync(token);
    }
}