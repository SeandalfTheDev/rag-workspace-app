namespace CleverDocs.Core.Abstractions.Documents;

public interface IDocumentProcessingService
{
    Task ProcessDocumentAsync(Guid documentId, CancellationToken token);
}