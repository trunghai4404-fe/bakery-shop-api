using Contract.DTOs;
using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Auth.Commands.Logout;

public record LogoutCommand( string RefreshToken) : IRequest<Result>;