using SharedKernel.Commons;
using SharedKernel.Domain.Errors;

namespace Contents.Domain.Errors;

public static class ContentsErrors
{
    public static readonly Error BannerNotFound = Error.NotFound(
        code: ErrorCode.BannerNotFound,
        description: ErrorMessage.BannerNotFound
    );
}
