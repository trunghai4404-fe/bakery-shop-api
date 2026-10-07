using Contents.Contracts.DTOs.Banners;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Queries.GetActiveBannersQuery;

public record GetActiveBannersQuery : IRequest<Result<List<BannerResponse>>>;
