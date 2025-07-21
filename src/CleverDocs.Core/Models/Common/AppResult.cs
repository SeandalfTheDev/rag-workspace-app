using CleverDocs.Core.Models.Common;

public class AppResult<T>
{
  private readonly T? _value;

  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public AppError Error { get; }

  public T Value
  {
    get
    {
        if (IsFailure)
        {
            throw new InvalidOperationException("there is no value for failure");
        }
        return _value!;
    }
    private init => _value = value;
  }

  private AppResult(T value)
  {
    _value = value;
    IsSuccess = true;
    Error = AppError.None;
  }

  private AppResult(AppError error)
  {
    if (error == AppError.None)
    {
      throw new ArgumentException("Error cannot be None", nameof(error));
    }

    Error = error;
    IsSuccess = false;
  }

  public static AppResult<T> Success(T value) => new(value);
  public static AppResult<T> Failure(AppError error) => new(error);
}