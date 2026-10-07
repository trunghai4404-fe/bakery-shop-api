using Contents.Contracts.DTOs.Banners;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Queries.GetActiveBannersQuery;

public class GetActiveBannersQueryHandler(
    IBannerRepository bannerRepository
) : IRequestHandler<GetActiveBannersQuery, Result<List<BannerResponse>>>
{
    public async Task<Result<List<BannerResponse>>> Handle(
        GetActiveBannersQuery request,
        CancellationToken ct)
    {
        var banners = await bannerRepository.GetActiveBannersAsync(ct);

        var responses = banners.Select(banner =>
        {
            var status = banner.GetStatus();
            return new BannerResponse
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
        }).ToList();

        return Result.Success(responses);
    }
}
