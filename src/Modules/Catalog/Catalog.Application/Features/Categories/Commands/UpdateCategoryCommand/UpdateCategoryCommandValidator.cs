using FluentValidation;

namespace Catalog.Application.Features.Categories.Commands.UpdateCategoryCommand;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(128).WithMessage("Category name cannot exceed 128 characters.");

        RuleFor(x => x.Slug)
            .MaximumLength(128).WithMessage("Category slug cannot exceed 128 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .When(x => !string.IsNullOrWhiteSpace(x.Slug))
            .WithMessage("Category slug must be in valid format (e.g. 'banh-ngot' using lowercase letters, numbers, and hyphens).");

        RuleFor(x => x.Description)
            .MaximumLength(256).WithMessage("Description cannot exceed 256 characters.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(1000).WithMessage("ImageUrl cannot exceed 1000 characters.");

        RuleFor(x => x.ParentId)
            .GreaterThan(0).When(x => x.ParentId.HasValue)
            .WithMessage("ParentId must be greater than 0.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("SortOrder must be greater than or equal to 0.");
    }
}
