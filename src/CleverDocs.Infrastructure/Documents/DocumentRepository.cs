using CleverDocs.Core.Abstractions.Documents;
using CleverDocs.Core.Entities;
using CleverDocs.Core.Models.Common;
using CleverDocs.Core.Models.Documents;
using CleverDocs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleverDocs.Infrastructure.Documents;

public class DocumentRepository(ApplicationDbContext context) : IDocumentRepository
{
    public async Task<PaginatedResult<Document>> GetDocumentsAsync(DocumentQuery query, CancellationToken token)
    {
        var queryable = context.Documents.AsQueryable();

        // Apply filters
        if (query.WorkspaceId.HasValue)
        {
            queryable = queryable.Where(d => d.WorkspaceId == query.WorkspaceId.Value);
        }

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            queryable = queryable.Where(d => 
                d.FileName.Contains(query.SearchTerm) || 
                d.OriginalFileName.Contains(query.SearchTerm));
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(d => d.DocumentStatus == query.Status.Value);
        }

        if (query.UploadedAfter.HasValue)
        {
            queryable = queryable.Where(d => d.UploadedAt >= query.UploadedAfter.Value);
        }

        if (query.UploadedBefore.HasValue)
        {
            queryable = queryable.Where(d => d.UploadedAt <= query.UploadedBefore.Value);
        }

        if (query.MaxFileSize.HasValue)
        {
            queryable = queryable.Where(d => d.FileSize <= query.MaxFileSize.Value);
        }

        if (query.FileExtensions != null && query.FileExtensions.Length > 0)
        {
            queryable = queryable.Where(d => query.FileExtensions.Contains(d.FileExtension));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(query.SortBy))
        {
            queryable = query.SortBy.ToLower() switch
            {
                "filename" => query.SortDescending 
                    ? queryable.OrderByDescending(d => d.FileName) 
                    : queryable.OrderBy(d => d.FileName),
                "originalfilename" => query.SortDescending 
                    ? queryable.OrderByDescending(d => d.OriginalFileName) 
                    : queryable.OrderBy(d => d.OriginalFileName),
                "filesize" => query.SortDescending 
                    ? queryable.OrderByDescending(d => d.FileSize) 
                    : queryable.OrderBy(d => d.FileSize),
                "uploadedat" => query.SortDescending 
                    ? queryable.OrderByDescending(d => d.UploadedAt) 
                    : queryable.OrderBy(d => d.UploadedAt),
                "createdat" => query.SortDescending 
                    ? queryable.OrderByDescending(d => d.CreatedAt) 
                    : queryable.OrderBy(d => d.CreatedAt),
                "status" => query.SortDescending 
                    ? queryable.OrderByDescending(d => d.DocumentStatus) 
                    : queryable.OrderBy(d => d.DocumentStatus),
                _ => queryable.OrderByDescending(d => d.CreatedAt) // Default sort
            };
        }
        else
        {
            // Default sorting by creation date descending
            queryable = queryable.OrderByDescending(d => d.CreatedAt);
        }

        // Get total count before pagination
        var totalCount = await queryable.CountAsync(token);

        // Apply pagination
        var skip = (query.PageNumber - 1) * query.PageSize;
        var documents = await queryable
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(token);

        return new PaginatedResult<Document>
        {
            Items = documents,
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    public async Task<Document?> GetDocumentAsync(Guid documentId, bool includeUploader = false, CancellationToken token = default)
    {
        return await context.Documents
            .FindAsync([documentId], cancellationToken: token);
    }

    public async Task<Document?> CreateDocumentAsync(Document document, CancellationToken token = default)
    {
        await context.Documents.AddAsync(document, cancellationToken: token);
        await context.SaveChangesAsync(token);
        return document;
    }

    public async Task<Document?> UpdateDocumentAsync(Document document, CancellationToken token = default)
    {
        context.Documents.Update(document);
        await context.SaveChangesAsync(token);
        return document;
    }

    public async Task DeleteDocumentAsync(Guid documentId, CancellationToken token = default)
    {
        await context.Documents
            .Where(doc => doc.Id == documentId)
            .ExecuteDeleteAsync(token);
    }
}