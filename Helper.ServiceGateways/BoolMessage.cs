namespace Helper.ServiceGateways;

public class BoolMessage
{
    public bool Value { get; set; }
    public string? Message { get; set; }

    public static BoolMessage True(string message) => new() { Value = true, Message = message, };
    public static BoolMessage False(string message) => new() { Value = false, Message = message, };

    public static implicit operator BoolMessage(bool value) => value ? True("True") : False("False");
}

public class OperationResult<T>(bool success, string? message = null, T? result = null) where T : class
{
    public bool Success { get; set; } = success;
    public string? Message { get; set; } = message;
    public T? Result { get; set; } = result;
    public long Milliseconds { get; set; }

    public static OperationResult<T> SuccessResult(T result, string? message = null) => new(true, message, result);
    public static OperationResult<T> Failure(string? message = null) => new(false, message);
}
