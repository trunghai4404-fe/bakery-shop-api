using Contract.DTOs;
using Contract.DTOs.Identity;
using MediatR;
using SharedKernel.Domain;

namespace Identity.Application.Features.Users.Queries.Profile;

public record GetProFileQuery : IRequest<Result<UserDto>> ;