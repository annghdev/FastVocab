namespace FastVocab.Shared.Utils;

/// <summary>
/// Represents the result of an operation with typed data
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public List<Error>? Errors { get; init; }

    private Result() { }

    /// <summary>
    /// Creates a successful result with data
    /// </summary>
    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    /// <summary>
    /// Creates a failed result with a single error message
    /// </summary>
    public static Result<T> Failure(Error errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = [errors]
        };
    }

    /// <summary>
    /// Creates a failed result with multiple error messages
    /// </summary>
    public static Result<T> Failure(List<Error> errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}

/// <summary>
/// Represents the result of an operation without data
/// </summary>
public class Result
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public List<Error>? Errors { get; init; }

    private Result() { }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success()
    {
        return new Result
        {
            IsSuccess = true
        };
    }

    /// <summary>
    /// Creates a failed result with a single error message
    /// </summary>
    public static Result Failure(Error error)
    {
        return new Result
        {
            IsSuccess = false,
            Errors = [error]
        };
    }

    /// <summary>
    /// Creates a failed result with multiple error messages
    /// </summary>
    public static Result Failure(List<Error> errors)
    {
        return new Result
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}

