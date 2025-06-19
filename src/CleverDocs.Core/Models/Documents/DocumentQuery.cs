using CleverDocs.Shared.Enums;

namespace CleverDocs.Core.Models.Documents;

public class DocumentQuery
{
    // Filtering
    public Guid? WorkspaceId { get; set; }
    public string? SearchTerm { get; set; }
    public DocumentStatus? Status { get; set; }
    public DateTime? UploadedAfter { get; set; }
    public DateTime? UploadedBefore { get; set; }
    public long? MaxFileSize { get; set; }
    public string[]? FileExtensions { get; set; }

    // Sorting
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;

    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    // Related data
    public bool IncludeUploader { get; set; } = false;
}
