using FluentValidation;

namespace Catalog.Application.Features.Categories.Commands.ToggleCategoryCommand;

public class ToggleCategoryCommandValidator : AbstractValidator<ToggleCategoryCommand>
{
    public ToggleCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0.");
    }
}
