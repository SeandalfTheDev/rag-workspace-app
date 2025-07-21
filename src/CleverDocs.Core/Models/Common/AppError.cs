namespace CleverDocs.Core.Models.Common;

public sealed record AppError(string Code, string Message)
{
  private static readonly string RecordNotFoundCode = "RECORD_NOT_FOUND";
  private static readonly string RecordAlreadyExistsCode = "RECORD_ALREADY_EXISTS";
  private static readonly string RecordNotUniqueCode = "RECORD_NOT_UNIQUE";
  private static readonly string ValidationErrorCode = "VALIDATION_ERROR";
  private static readonly string InvalidOperationCode = "INVALID_OPERATION";
  private static readonly string UnauthorizedCode = "UNAUTHORIZED";
  private static readonly string ForbiddenCode = "FORBIDDEN";

  public static readonly AppError None = new(string.Empty, string.Empty);

  public static AppError Generic(string message) => new(string.Empty, message); 
  public static AppError RecordNotFound(string message) => new(RecordNotFoundCode, message);
  public static AppError RecordAlreadyExists(string message) => new(RecordAlreadyExistsCode, message);
  public static AppError RecordNotUnique(string message) => new(RecordNotUniqueCode, message);
  public static AppError ValidationError(string message) => new(ValidationErrorCode, message);
  public static AppError InvalidOperation(string message) => new(InvalidOperationCode, message);
  public static AppError Unauthorized(string message) => new(UnauthorizedCode, message);
  public static AppError Forbidden(string message) => new(ForbiddenCode, message);
}