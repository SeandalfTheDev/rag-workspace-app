namespace CleverDocs.Shared.Abstractions;

public class Result<T> where T : class
{
    public T? Data { get; set; }
    public string? Error { get; set; }
    public bool IsSuccess { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Data = data, IsSuccess = true };
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T> { Error = error, IsSuccess = false };
    }  
}