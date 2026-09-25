namespace Identity.Domain.Errors;

public static class ErrorCode
{
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
    public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
    public const string AccountInactive = "ACCOUNT_INACTIVE";
    
    //Role
    public const string RoleAlreadyExists = "ROLE_ALREADY_EXISTS";
    public const string RoleNotFound = "ROLE_NOT_FOUND";
    public const string InvalidIdentifier = "INVALID_IDENTIFIER";
}