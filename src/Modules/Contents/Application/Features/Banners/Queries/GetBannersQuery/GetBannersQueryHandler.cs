using Contents.Contracts.DTOs.Banners;
using Contents.Domain.Repositories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Queries.GetBannersQuery;

public class GetBannersQueryHandler(
    IBannerRepository bannerRepository
) : IRequestHandler<GetBannersQuery, Result<PagedList<BannerResponse>>>
{
    public async Task<Result<PagedList<BannerResponse>>> Handle(
        GetBannersQuery request,
        CancellationToken ct)
    {
        var pagedBanners = await bannerRepository.GetBannersAsync(
            request.Search,
            request.IsActive,
            request.Status,
            request.Page,
            request.PageSize,
            ct);

        var dtos = pagedBanners.Items.Select(banner =>
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

        var response = new PagedList<BannerResponse>(
            dtos,
            pagedBanners.Page,
            pagedBanners.PageSize,
            pagedBanners.TotalCount
        );

        return Result.Success(response);
    }
}
