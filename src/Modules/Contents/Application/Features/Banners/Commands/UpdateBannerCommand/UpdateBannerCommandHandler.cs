using Contents.Contracts.DTOs.Banners;
using Contents.Domain.Errors;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Commands.UpdateBannerCommand;

public class UpdateBannerCommandHandler(
    IBannerRepository bannerRepository,
    IContentsUnitOfWork unitOfWork
) : IRequestHandler<UpdateBannerCommand, Result<BannerResponse>>
{
    public async Task<Result<BannerResponse>> Handle(
        UpdateBannerCommand request,
        CancellationToken ct)
    {
        var banner = await bannerRepository.GetByIdAsync(request.Id, ct);
        if (banner is null)
        {
            return Result.Failure<BannerResponse>(ContentsErrors.BannerNotFound);
        }

        var updateResult = banner.Update(
            title: request.Title,
            imageUrl: request.ImageUrl,
            mobileImageUrl: request.MobileImageUrl,
            actionType: request.ActionType,
            targetValue: request.TargetValue,
            openInNewTab: request.OpenInNewTab,
            isActive: request.IsActive,
            startDate: request.StartDate,
            endDate: request.EndDate,
            metadata: request.Metadata
        );

        if (updateResult.IsFailure)
        {
            return Result.Failure<BannerResponse>(updateResult.Error);
        }

        bannerRepository.Update(banner);
        await unitOfWork.SaveChangesAsync(ct);

        var status = banner.GetStatus();

        var response = new BannerResponse
        {
            Id = banner.Id,
            Title = banner.Title,
            ImageUrl = banner.ImageUrl,
            MobileImageUrl = banner.MobileImageUrl,
            ActionType = banner.ActionType.ToString(),
            ActionTypeValue = (int)banner.ActionType,
            TargetValue = banner.TargetValue,
            OpenInNewTab = banner.OpenInNewTab,
            IsActive = banner.IsActive,
            Status = status.ToString(),
            StatusValue = (int)status,
            StartDate = banner.StartDate,
            EndDate = banner.EndDate,
            Metadata = banner.Metadata,
            CreatedAt = banner.CreatedAt,
            LastModifiedAt = banner.LastModifiedAt
        };

        return Result.Success(response);
    }
}
