using MediatR;
using SharedKernel.Domain;

namespace Contents.Application.Features.Banners.Commands.DeleteBannerCommand;

public record DeleteBannerCommand(Guid Id) : IRequest<Result>;
