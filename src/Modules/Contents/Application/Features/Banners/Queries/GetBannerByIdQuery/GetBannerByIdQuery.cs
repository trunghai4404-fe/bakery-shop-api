using Contents.Contracts.DTOs.Banners;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Queries.GetBannerByIdQuery;

public record GetBannerByIdQuery(Guid Id) : IRequest<Result<BannerResponse>>;
