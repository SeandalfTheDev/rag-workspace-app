namespace CleverDocs.Core.Entities;

//document_chunks (id, document_id, chunk_index, content, embedding[vector], metadata, created_at)

public class DocumentChunk
{
  public Guid DocumentId { get; set;}
  public int ChunkIndex { get; set;}
  public string Content { get; set;} = string.Empty;
  public float[] Embedding { get; set;} = Array.Empty<float>();
  public Dictionary<string, string> Metadata { get; set;} = new Dictionary<string, string>();
  public DateTime CreatedAt { get; set;} = DateTime.UtcNow;
}