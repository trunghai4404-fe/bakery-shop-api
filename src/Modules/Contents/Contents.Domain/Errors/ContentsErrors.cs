using SharedKernel.Commons;
using SharedKernel.Domain.Errors;

namespace Contents.Domain.Errors;

public static class ContentsErrors
{
    public static readonly Error ContactNotFound =
        Error.NotFound("CONTACT_NOT_FOUND", "Contact not found.");

    public static readonly Error InvalidContactType =
        Error.Validation("INVALID_CONTACT_TYPE", "Contact type must be Email, Zalo, Facebook, Phone, or TikTok.");

    public static readonly Error InvalidContactValue =
        Error.Validation("INVALID_CONTACT_VALUE", "Contact value is required and must match its contact type.");

    public static readonly Error StoreNotFound =
        Error.NotFound(ErrorCode.StoreNotFound, ErrorMessage.StoreNotFound);

    public static readonly Error BannerNotFound = Error.NotFound(
        code: ErrorCode.BannerNotFound,
        description: ErrorMessage.BannerNotFound
    );
}
