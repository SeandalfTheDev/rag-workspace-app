using CleverDocs.Shared.Enums;

namespace CleverDocs.Core.Abstractions.Notifications;

public interface ISignalRNotificationService
{
    Task NotifyDocumentProcessingStatusAsync(Guid documentId, DocumentStatus status, string message);
}