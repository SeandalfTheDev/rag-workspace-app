namespace CleverDocs.Core.Abstractions.Documents;

public interface IChunkingService
{
    List<string> CreateChunks(string text);
}