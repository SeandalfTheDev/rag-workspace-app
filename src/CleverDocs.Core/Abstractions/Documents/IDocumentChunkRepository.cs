using CleverDocs.Core.Entities;

namespace CleverDocs.Core.Abstractions.Documents;

public interface IDocumentChunkRepository
{
    Task<List<DocumentChunk>> GetByDocumentIdAsync(Guid documentId, CancellationToken token = default);
    Task<List<DocumentChunk>> GetSimilarAsync(float[] embedding, CancellationToken token = default);
    Task<DocumentChunk?> GetChunkAsync(Guid documentId, int index, CancellationToken token = default);
    Task<DocumentChunk?> AddAsync(DocumentChunk documentChunk, CancellationToken token = default);
    Task AddBatchAsync(List<DocumentChunk> documentChunks, CancellationToken token = default);
    Task<DocumentChunk?> UpdateAsync(DocumentChunk documentChunk, CancellationToken token = default);
    Task DeleteChunkAsync(Guid documentId, int index, CancellationToken token = default);
    Task DeleteAllChunksForDocumentAsync(Guid documentId, CancellationToken token = default);
}