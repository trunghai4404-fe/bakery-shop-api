using Contract.DTOs.Identity;
using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken) : IRequest<Result<JwtResponse>>;