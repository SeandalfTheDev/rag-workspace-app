using CleverDocs.Core.Abstractions.Documents;
using CleverDocs.Core.Abstractions.Notifications;
using CleverDocs.Core.Entities;
using CleverDocs.Shared.Enums;
using Serilog;

namespace CleverDocs.Infrastructure.Documents;

public class DocumentProcessingService : IDocumentProcessingService
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentChunkRepository _documentChunkRepository;
    private readonly ITextExtractionService _textExtractionService;
    private readonly IChunkingService _chunkService;
    private readonly IEmbeddingService _embeddingService;
    private readonly ISignalRNotificationService  _notificationService;
    private readonly Serilog.ILogger _logger;
    
    public DocumentProcessingService(IDocumentRepository documentRepository,
        IDocumentChunkRepository documentChunkRepository, ITextExtractionService textExtractionService,
        IChunkingService chunkService, IEmbeddingService embeddingService,
        ISignalRNotificationService notificationService, ILogger logger)
    {
        _documentRepository = documentRepository ?? throw new ArgumentNullException(nameof(documentRepository));
        _documentChunkRepository =
            documentChunkRepository ?? throw new ArgumentNullException(nameof(documentChunkRepository));
        _textExtractionService =
            textExtractionService ?? throw new ArgumentNullException(nameof(textExtractionService));
        _chunkService = chunkService ?? throw new ArgumentNullException(nameof(chunkService));
        _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task ProcessDocumentAsync(Guid documentId, CancellationToken token)
    {
        try
        {
            var document = await  _documentRepository.GetDocumentAsync(documentId, token: token);
            if (document is null)
            {
                _logger.Warning("Document with ID {DocumentId} was not found", documentId);
                return;
            }
            
            // === STAGE 1: TEXT EXTRACTION ===
            await _notificationService.NotifyDocumentProcessingStatusAsync(documentId, DocumentStatus.Processing, "Extracting text...");
            var content  = await _textExtractionService.ExtractTextAsync(document.FilePath, document.ContentType);
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidDataException("Extracted text content was empty.");
            }
            
            // === STAGE 2: TEXT CHUNKING ===
            await _notificationService.NotifyDocumentProcessingStatusAsync(documentId, DocumentStatus.Processing, "Splitting document into chunks...");
            var chunks = _chunkService.CreateChunks(content);
            
            // === STAGE 3: EMBEDDING GENERATION ===
            await _notificationService.NotifyDocumentProcessingStatusAsync(documentId, DocumentStatus.Processing, "Generating AI embeddings...");
            var embeddings = await _embeddingService.GenerateEmbeddingsAsync(chunks, token);
            
            // === STAGE 4: PERSISTENCE ===
            await _notificationService.NotifyDocumentProcessingStatusAsync(documentId, DocumentStatus.Processing, "Saving to knowledge base...");
            var documentChunks = chunks.Zip(embeddings, (text, vector) => new DocumentChunk()
            {
                DocumentId = document.Id,
                Content = text,
                Embedding = vector
            }).ToList();

            await _documentChunkRepository.AddBatchAsync(documentChunks, token);
            
            // === STAGE 5: FINALIZATION ===
            document.DocumentStatus = DocumentStatus.Completed;
            document.ProcessedAt = DateTime.UtcNow;
            
            await _documentRepository.UpdateDocumentAsync(document, token);
            
            await _notificationService.NotifyDocumentProcessingStatusAsync(documentId, DocumentStatus.Completed, "Processing complete! You can now chat with your document.");
            _logger.Information("Successfully proceed document {DocumentId}", document.Id);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to process document {DocumentId}", documentId);
            
            // Update status to Failed and notify user
            var document = await _documentRepository.GetDocumentAsync(documentId);
            if (document != null)
            {
                document.DocumentStatus = DocumentStatus.Failed;
                await _documentRepository.UpdateDocumentAsync(document);
                await _notificationService.NotifyDocumentProcessingStatusAsync(documentId, DocumentStatus.Failed, $"An error occurred: {ex.Message}");
            }
        }
    }
}