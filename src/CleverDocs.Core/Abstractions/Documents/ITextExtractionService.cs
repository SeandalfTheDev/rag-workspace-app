namespace CleverDocs.Core.Abstractions.Documents;

public interface ITextExtractionService
{
    Task<string> ExtractTextAsync(string filePath, string contentType);
}