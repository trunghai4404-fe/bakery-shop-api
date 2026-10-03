using FluentValidation;

namespace Catalog.Application.Features.Products.Queries.GetProductBySlugQuery;

public class GetProductBySlugQueryValidator : AbstractValidator<GetProductBySlugQuery>
{
    public GetProductBySlugQueryValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Product slug is required.")
            .MaximumLength(256).WithMessage("Product slug cannot exceed 256 characters.");
    }
}
