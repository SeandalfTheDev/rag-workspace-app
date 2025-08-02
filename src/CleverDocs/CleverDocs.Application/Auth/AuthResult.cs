using CleverDocs.Domain.Auth;

namespace CleverDocs.Application.Auth;

/// <summary>
/// Represents the result of an authentication operation
/// </summary>
/// <typeparam name="T">The type of the result data</typeparam>
public class AuthResult<T>
{
    public bool Succeeded { get; set; }
    public T? Data { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
    public string? ErrorMessage { get; set; }
    public AuthErrorType ErrorType { get; set; }

    public static AuthResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public static AuthResult<T> Failure(string errorMessage, IDictionary<string, string[]>? errors = null, AuthErrorType errorType = AuthErrorType.ServerError) => 
        new() { Succeeded = false, ErrorMessage = errorMessage, Errors = errors, ErrorType = errorType };
}