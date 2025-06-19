namespace CleverDocs.Core.Abstractions.Documents;

public interface IEmbeddingService
{
    Task<List<float[]>> GenerateEmbeddingsAsync(List<string> chunks, CancellationToken token);
}