using FluentValidation;

namespace Catalog.Application.Features.Categories.Commands.DeleteCategoryCommand;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0.");
    }
}
