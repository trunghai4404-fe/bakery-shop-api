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
    
    //Identity
    public const string UserNotFound = "USER_NOT_FOUND";
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";
    public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
    public const string AccountInactive = "ACCOUNT_INACTIVE";
    
    //Role
    public const string RoleAlreadyExists = "ROLE_ALREADY_EXISTS";
    public const string RoleNotFound = "ROLE_NOT_FOUND";
    public const string InvalidIdentifier ="Id or Code is required.";
    
    //Catalog
    public const string VariantNotFound = "VARIANT_NOT_FOUND";
    public const string ProductNotFound = "PRODUCT_NOT_FOUND";
    public const string ProductVariantNotFound = "PRODUCT_VARIANT_NOT_FOUND";
    public const string ProductSlugAlreadyExists = "PRODUCT_SLUG_ALREADY_EXISTS";
    public const string ProductSkuAlreadyExists = "PRODUCT_SKU_ALREADY_EXISTS";
    public const string CategoryNotFound = "CATEGORY_NOT_FOUND";
    public const string CategorySlugAlreadyExists = "CATEGORY_SLUG_ALREADY_EXISTS";
    public const string ParentCategoryNotFound = "PARENT_CATEGORY_NOT_FOUND";
    public const string CircularCategoryParent = "CIRCULAR_CATEGORY_PARENT";
    public const string CategoryHasProducts = "CATEGORY_HAS_PRODUCTS";
    public const string CategoryHasChildren = "CATEGORY_HAS_CHILDREN";
}
