using SharedKernel.Commons;

namespace SharedKernel.Domain.Errors;
public record Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new(ErrorCode.NullValue, "Null value is invalid.", ErrorType.Failure);
    public static readonly Error InternalServerError = new(ErrorCode.InternalServerError, "An internal server error occurred.", ErrorType.Failure);
    public static readonly Error Unauthorized = new(ErrorCode.Unauthorized, "User is unauthorized or session has expired.", ErrorType.Unauthorized);
    public static  Error Forbidden(string code = ErrorCode.Forbidden, string description = "You do not have permission")
        => new(code, description, ErrorType.Forbidden);

    public static Error NotFound(string code = ErrorCode.NotFound, string description = "The requested resource was not found.") 
        => new(code, description, ErrorType.NotFound);

    public static Error Validation(string code = ErrorCode.ValidationError, string description = "One or more validation errors occurred.") 
        => new(code, description, ErrorType.Validation);

    public static Error Conflict(string code = ErrorCode.AlreadyExists, string description = "A conflict occurred or the resource already exists.") 
        => new(code, description, ErrorType.Conflict);

    public static Error Failure(string code = ErrorCode.OperationFailed, string description = "The requested operation failed.") 
        => new(code, description, ErrorType.Failure);
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5
}
