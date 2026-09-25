namespace Identity.Domain.Errors;

public class ErrorMessage
{
    public const string UserNotFound = "User not found";
    public const string InvalidCredentials = "Invalid email or password";
    public const string EmailAlreadyExists = "Email already exists";
    public const string InvalidRefreshToken = "Invalid refresh token";
    public const string AccountInactive = "Account is inactive";
    
    //Role
    public const string RoleAlreadyExists = "Role already exists";
    public const string RoleNotFound = "Role is not found";
    public const string InvalidIdentifier ="Id or Code is required.";
}