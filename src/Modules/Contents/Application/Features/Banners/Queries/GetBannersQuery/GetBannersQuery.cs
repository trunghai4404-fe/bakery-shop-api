using Contents.Contracts.DTOs.Banners;
using Contents.Domain.Enums;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Queries.GetBannersQuery;

public record GetBannersQuery(
    string? Search,
    bool? IsActive,
    BannerStatus? Status,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PagedList<BannerResponse>>>;
