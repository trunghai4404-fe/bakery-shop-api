using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password
    ) : IRequest<Result>;