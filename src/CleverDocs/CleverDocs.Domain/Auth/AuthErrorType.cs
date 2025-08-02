namespace CleverDocs.Domain.Auth;

public enum AuthErrorType
{
    /// <summary>
    /// Represents a validation failure, typically corresponding to a 400 Bad Request.
    /// </summary>
    Validation,
    
    /// <summary>
    /// Represents a conflict, such as a user already existing, corresponding to a 409 Conflict.
    /// </summary>
    Conflict,
    
    /// <summary>
    /// Represents a generic server failure, corresponding to a 500 Internal Server Error.
    /// </summary>
    ServerError
}