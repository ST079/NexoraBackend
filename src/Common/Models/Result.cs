namespace NexoraBackend.Common.Models;

public class Result<T>
{
    //used when we want to return a value along with the success/failure status of an operation
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public Error? Error { get; private set; }

    private Result() { }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static Result<T> Failure(Error error) =>
        new() { IsSuccess = false, Error = error };

    public static Result<T> Failure(string code, string message) =>
        Failure(new Error(code, message));

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(Value!) : onFailure(Error!);
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public Error? Error { get; private set; }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(Error error) => new() { IsSuccess = false, Error = error };
    public static Result Failure(string code, string message) =>
        Failure(new Error(code, message));
}