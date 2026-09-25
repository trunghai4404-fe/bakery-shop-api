namespace SharedKernel.Commons;
public static class ErrorCode
{
    // Common / System Errors
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string BadRequest = "BAD_REQUEST";
    public const string NotFound = "NOT_FOUND";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotHavePermission = "NOT_HAVE_PERMISSION";
    
    // Data & Validation Errors
    public const string InvalidValue = "INVALID_VALUE";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string AlreadyExists = "ALREADY_EXISTS";
    public const string NullValue = "NULL_VALUE";
    public const string Conflict = "CONFLICT";
    public const string OperationFailed = "OPERATION_FAILED";
    
    // Auth Errors
    public const string ExpiredToken = "EXPIRED_TOKEN";
    public const string InvalidToken = "INVALID_TOKEN";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
}
