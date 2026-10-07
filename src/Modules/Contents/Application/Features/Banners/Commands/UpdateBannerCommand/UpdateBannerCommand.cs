using Contents.Contracts.DTOs.Banners;
using Contents.Domain.Enums;
using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Commands.UpdateBannerCommand;

public record UpdateBannerCommand(
    Guid Id,
    string Title,
    string ImageUrl,
    string? MobileImageUrl,
    BannerActionType ActionType,
    string? TargetValue,
    bool OpenInNewTab,
    bool IsActive,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string? Metadata) : IRequest<Result<BannerResponse>>;
