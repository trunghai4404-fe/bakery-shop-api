using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Commands.DeleteCategoryCommand;

public record DeleteCategoryCommand(long Id) : IRequest<Result>;
