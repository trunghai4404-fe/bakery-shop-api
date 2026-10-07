using Contents.Contracts.DTOs.Banners;
using Contents.Domain.Entities;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Commands.CreateBannerCommand;

public class CreateBannerCommandHandler(
    IBannerRepository bannerRepository,
    IContentsUnitOfWork unitOfWork
) : IRequestHandler<CreateBannerCommand, Result<BannerResponse>>
{
    public async Task<Result<BannerResponse>> Handle(
        CreateBannerCommand request,
        CancellationToken ct)
    {
        var bannerResult = Banner.Create(
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

        if (bannerResult.IsFailure)
        {
            return Result.Failure<BannerResponse>(bannerResult.Error);
        }

        var banner = bannerResult.Value;

        await bannerRepository.AddAsync(banner, ct);
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
