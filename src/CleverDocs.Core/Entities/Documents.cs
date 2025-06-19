using CleverDocs.Shared.Enums;

namespace CleverDocs.Core.Entities;

public class Document
{
  public Guid Id { get; set;}
  public Guid WorkspaceId { get; set;}
  public string ObjectId { get; set;} = string.Empty;
  public string FileName { get; set;} = string.Empty;
  public string OriginalFileName { get; set;} = string.Empty;
  public string ContentType { get; set;} = string.Empty;
  public long FileSize { get; set;} = 0;
  public string FilePath { get; set;} = string.Empty;
  public string FileExtension { get; set;} = string.Empty;
  public DocumentStatus DocumentStatus { get; set;} = DocumentStatus.Pending;
  public Guid UploadedBy { get; set;}
  public DateTime UploadedAt { get; set;} = DateTime.UtcNow;
  public DateTime? ProcessedAt { get; set;}
  public DateTime CreatedAt { get; set;} = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;
}