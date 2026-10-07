using Contents.Domain.Errors;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Commands.DeleteBannerCommand;

public class DeleteBannerCommandHandler(
    IBannerRepository bannerRepository,
    IContentsUnitOfWork unitOfWork
) : IRequestHandler<DeleteBannerCommand, Result>
{
    public async Task<Result> Handle(
        DeleteBannerCommand request,
        CancellationToken ct)
    {
        var banner = await bannerRepository.GetByIdAsync(request.Id, ct);
        if (banner is null)
        {
            return Result.Failure(ContentsErrors.BannerNotFound);
        }

        bannerRepository.Delete(banner);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
