using FluentValidation;

namespace Catalog.Application.Features.Products.Commands.ToggleProductCommand;

public class ToggleProductCommandValidator : AbstractValidator<ToggleProductCommand>
{
    public ToggleProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Product ID must be greater than 0.");
    }
}
