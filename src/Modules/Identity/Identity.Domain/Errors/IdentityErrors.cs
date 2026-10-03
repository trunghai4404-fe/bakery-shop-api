using SharedKernel.Commons;
using SharedKernel.Domain.Errors;

namespace Identity.Domain.Errors;

public static class IdentityErrors
{
    public static readonly Error UserNotFound = Error.NotFound(
        code: ErrorCode.UserNotFound,
        description: ErrorMessage.UserNotFound 
        );
    public static readonly Error InvalidCredentials = Error.Validation(
        code: ErrorCode.InvalidCredentials,
        description: ErrorMessage.InvalidCredentials);
    public static readonly Error EmailAlreadyExists = Error.Validation(
        code: ErrorCode.EmailAlreadyExists,
        description: ErrorMessage.EmailAlreadyExists
    );
    public static readonly Error InvalidRefreshToken = Error.Validation(
        code: ErrorCode.InvalidRefreshToken,
        description: ErrorMessage.InvalidRefreshToken
    );
    public static readonly Error AccountInActive = Error.Forbidden(
        code: ErrorCode.AccountInactive,
        description: ErrorMessage.AccountInactive
    );
    // Role
    public static readonly Error RoleAlreadyExists = Error.Validation(
        code: ErrorCode.RoleAlreadyExists,
        description: ErrorMessage.RoleAlreadyExists
    );

    public static readonly Error RoleNotFound = Error.NotFound(
        code: ErrorCode.RoleNotFound,
        description: ErrorMessage.RoleNotFound
    );

    public static readonly Error InvalidIdentifier = Error.Validation(
        code: ErrorCode.InvalidIdentifier,
        description: ErrorMessage.InvalidIdentifier
    );
}