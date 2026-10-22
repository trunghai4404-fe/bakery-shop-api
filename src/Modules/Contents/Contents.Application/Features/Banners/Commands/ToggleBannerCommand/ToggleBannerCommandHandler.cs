using Contents.Domain.Errors;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Commands.ToggleBannerCommand;

public class ToggleBannerCommandHandler(
    IBannerRepository bannerRepository,
    IContentsUnitOfWork unitOfWork
) : IRequestHandler<ToggleBannerCommand, Result>
{
    public async Task<Result> Handle(
        ToggleBannerCommand request,
        CancellationToken ct)
    {
        var banner = await bannerRepository.GetByIdAsync(request.Id, ct);
        if (banner is null)
        {
            return Result.Failure(ContentsErrors.BannerNotFound);
        }

        banner.ToggleStatus(request.IsActive);
        bannerRepository.Update(banner);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
