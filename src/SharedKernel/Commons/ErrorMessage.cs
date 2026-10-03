namespace SharedKernel.Commons;

public class ErrorMessage
{
    // Systems
    public const string NullValue = "Null value is invalid.";
    public const string InternalServerError = "Internal Server Error";
    public const string Unauthorized = "User is unauthorized or session has expired.";
    public const string Forbidden = "You do not have permission";
    public const string Validation = "One or more validation errors occurred.";
    public const string NotFound = "Resource not found";
    public const string Conflict = "A conflict occurred or the resource already exists.";
    public const string Failure = "The requested operation failed.";
    
    // Identity
    public const string UserNotFound = "User not found";
    public const string InvalidCredentials = "Invalid email or password";
    public const string EmailAlreadyExists = "Email already exists";
    public const string InvalidRefreshToken = "Invalid refresh token";
    public const string AccountInactive = "Account is inactive";
    
    // Role
    public const string RoleAlreadyExists = "Role already exists";
    public const string RoleNotFound = "Role is not found";
    public const string InvalidIdentifier ="Id or Code is required.";
    
    // Catalog
    public const string CategoryNotFound = "Category not found.";
    public const string CategorySlugAlreadyExists = "Category slug already exists.";
    public const string ParentCategoryNotFound = "Parent category not found.";
    public const string CircularCategoryParent = "A category cannot be set as a child or descendant of itself.";
    public const string ProductNotFound = "Product not found.";
    public const string ProductVariantNotFound = "Product variant not found.";
    public const string CategoryHasProducts = "Cannot delete category because it still contains products.";
    public const string CategoryHasChildren = "Cannot delete category because it still contains child categories.";
}
