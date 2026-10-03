using SharedKernel.Commons;
using SharedKernel.Domain.Errors;

namespace Catalog.Domain.Errors;

public static class CatalogErrors
{
    public static readonly Error CategoryNotFound = Error.NotFound(
        code: ErrorCode.CategoryNotFound,
        description: ErrorMessage.CategoryNotFound
    );

    public static readonly Error CategorySlugAlreadyExists = Error.Conflict(
        code: ErrorCode.CategorySlugAlreadyExists,
        description: ErrorMessage.CategorySlugAlreadyExists
    );

    public static readonly Error ParentCategoryNotFound = Error.NotFound(
        code: ErrorCode.ParentCategoryNotFound,
        description: ErrorMessage.ParentCategoryNotFound
    );

    public static readonly Error CircularCategoryParent = Error.Conflict(
        code: ErrorCode.CircularCategoryParent,
        description: ErrorMessage.CircularCategoryParent
    );

    public static readonly Error CategoryHasProducts = Error.Conflict(
        code: ErrorCode.CategoryHasProducts,
        description: ErrorMessage.CategoryHasProducts
    );

    public static readonly Error CategoryHasChildren = Error.Conflict(
        code: ErrorCode.CategoryHasChildren,
        description: ErrorMessage.CategoryHasChildren
    );

    public static readonly Error ProductNotFound = Error.NotFound(
        code: ErrorCode.ProductNotFound,
        description: ErrorMessage.ProductNotFound
    );

    public static readonly Error ProductVariantNotFound = Error.NotFound(
        code: ErrorCode.ProductVariantNotFound,
        description: ErrorMessage.ProductVariantNotFound
    );

    public static readonly Error ProductSlugAlreadyExists = Error.Conflict(
        code: ErrorCode.ProductSlugAlreadyExists,
        description: ErrorMessage.ProductSlugAlreadyExists
    );

    public static readonly Error ProductSkuAlreadyExists = Error.Conflict(
        code: ErrorCode.ProductSkuAlreadyExists,
        description: ErrorMessage.ProductSkuAlreadyExists
    );
}
