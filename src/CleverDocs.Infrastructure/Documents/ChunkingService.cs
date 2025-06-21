using System.Diagnostics.CodeAnalysis;
using CleverDocs.Core.Abstractions.Documents;
using Microsoft.SemanticKernel.Text;

namespace CleverDocs.Infrastructure.Documents;

public class ChunkingService : IChunkingService
{
    private const int ChunkSize = 2000;
    private const int ChunkOverlap = 200;
    
    [Experimental("SKEXP0050")]
    public List<string> CreateChunks(string text)
    {
        var lines = TextChunker.SplitPlainTextLines(text, 40);
        var paragraphs = TextChunker.SplitPlainTextParagraphs(lines, 30);
        return paragraphs;
    }
}