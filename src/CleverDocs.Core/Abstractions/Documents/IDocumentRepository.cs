using CleverDocs.Core.Entities;
using CleverDocs.Core.Models.Common;
using CleverDocs.Core.Models.Documents;

namespace CleverDocs.Core.Abstractions.Documents;

public interface IDocumentRepository
{
    Task<PaginatedResult<Document>> GetDocumentsAsync(DocumentQuery query, CancellationToken token);
    Task<Document?> GetDocumentAsync(Guid documentId, bool includeUploader = false, CancellationToken token = default);
    Task<Document?> CreateDocumentAsync(Document document, CancellationToken token = default);
    Task<Document?> UpdateDocumentAsync(Document document, CancellationToken token = default);
    Task DeleteDocumentAsync(Guid documentId, CancellationToken token = default);
}