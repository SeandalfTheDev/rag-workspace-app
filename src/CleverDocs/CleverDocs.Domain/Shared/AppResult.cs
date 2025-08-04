namespace CleverDocs.Domain.Shared;

/// <summary>
/// Represents the outcome of an operation, which can be either a success or a failure.
/// This is the base, non-generic result class.
/// </summary>
public class AppResult
{
    protected internal AppResult(bool isSuccess, AppError error)
    {
        // Enforce the invariant that a success result cannot have an error,
        // and a failure result must have an error.
        if (isSuccess && error != AppError.None)
        {
            throw new InvalidOperationException(
                "A success result cannot contain an error."
            );
        }

        if (!isSuccess && error == AppError.None)
        {
            throw new InvalidOperationException(
                "A failure result must contain an error."
            );
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public AppError Error { get; }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static AppResult Success() => new(true, AppError.None);

    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    public static AppResult Failure(AppError error) => new(false, error);

    /// <summary>
    /// Creates a success result with a value.
    /// </summary>
    public static AppResult<TValue> Success<TValue>(TValue value) =>
        new(value, true, AppError.None);
    
    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    public static AppResult<TValue> Failure<TValue>(AppError error) =>
        new(default, false, error);
}

/// <summary>
/// Represents the outcome of an operation that returns a value.
/// </summary>
/// <typeparam name="TValue">The type of the value returned on success.</typeparam>
public class AppResult<TValue> : AppResult
{
    private readonly TValue? _value;

    protected internal AppResult(TValue? value, bool isSuccess, AppError error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value of the result.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when trying to access the value of a failure result.
    /// </exception>
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                "Cannot access the value of a failure result."
            );

    /// <summary>
    /// Provides a way to implicitly convert a value to a success result.
    /// </summary>
    public static implicit operator AppResult<TValue>(TValue value) => Success(value);

    /// <summary>
    /// Provides a way to implicitly convert an error to a failure result.
    /// </summary>
    public static implicit operator AppResult<TValue>(AppError error) =>
        new(default, false, error);

    /// <summary>
    /// Executes one of the provided functions based on the result's state.
    /// </summary>
    /// <param name="onSuccess">The function to execute if the result is a success.</param>
    /// <param name="onFailure">The function to execute if the result is a failure.</param>
    /// <returns>The return value of the executed function.</returns>
    public TOut Match<TOut>(
        Func<TValue, TOut> onSuccess,
        Func<AppError, TOut> onFailure
    ) => IsSuccess ? onSuccess(Value) : onFailure(Error);
}