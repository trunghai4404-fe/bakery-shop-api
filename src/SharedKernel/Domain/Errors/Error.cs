using SharedKernel.Commons;

namespace SharedKernel.Domain.Errors;
public record Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new(ErrorCode.NullValue, ErrorMessage.NullValue, ErrorType.Failure);
    public static readonly Error InternalServerError = new(ErrorCode.InternalServerError, ErrorMessage.InternalServerError, ErrorType.Failure);
    public static readonly Error Unauthorized = new(ErrorCode.Unauthorized, ErrorMessage.Unauthorized, ErrorType.Unauthorized);
    public static  Error Forbidden(string code = ErrorCode.Forbidden, string description = ErrorMessage.Forbidden)
        => new(code, description, ErrorType.Forbidden);

    public static Error NotFound(string code = ErrorCode.NotFound, string description = ErrorMessage.NotFound) 
        => new(code, description, ErrorType.NotFound);

    public static Error Validation(string code = ErrorCode.ValidationError, string description =  ErrorMessage.Validation) 
        => new(code, description, ErrorType.Validation);

    public static Error Conflict(string code = ErrorCode.AlreadyExists, string description =  ErrorMessage.Conflict) 
        => new(code, description, ErrorType.Conflict);

    public static Error Failure(string code = ErrorCode.OperationFailed, string description = ErrorMessage.Failure) 
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
