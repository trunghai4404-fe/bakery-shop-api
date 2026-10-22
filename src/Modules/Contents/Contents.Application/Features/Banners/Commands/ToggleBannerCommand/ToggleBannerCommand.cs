using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Commands.ToggleBannerCommand;

public record ToggleBannerCommand(Guid Id, bool? IsActive = null) : IRequest<Result>;
