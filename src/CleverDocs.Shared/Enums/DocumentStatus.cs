namespace CleverDocs.Shared.Enums;

/// <summary>
/// The status of the document upload.
/// </summary>
public enum DocumentStatus
{
  /// <summary>
  /// The document is pending upload.
  /// </summary>
  Pending,
  /// <summary>
  /// The document is being uploaded.
  /// </summary>
  Uploading,
  /// <summary>
  /// The document is being processed.
  /// </summary>
  Processing,
  /// <summary>
  /// The document has been completed.
  /// </summary>
  Completed,
  /// <summary>
  /// The document has failed to upload.
  /// </summary>
  Failed
}