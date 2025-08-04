namespace CleverDocs.Domain.Shared;

/// <summary>
/// Represents a specific error with a code and a description.
/// </summary>
/// <param name="Code">An identifier for the error (e.g., "User.NotFound").</param>
/// <param name="Description">A developer-facing description of the error.</param>
public record AppError(string Code, string Description)
{
    /// <summary>
    /// A representation of no error.
    /// </summary>
    public static readonly AppError None = new(string.Empty, string.Empty);
}