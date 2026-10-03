using FluentValidation;

namespace Catalog.Application.Features.Products.Queries.GetProductsQuery;

public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize must be greater than or equal to 1.")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).When(x => x.CategoryId.HasValue)
            .WithMessage("CategoryId must be greater than 0.");

        RuleFor(x => x.Search)
            .MaximumLength(100).WithMessage("Search keyword cannot exceed 100 characters.");

        RuleFor(x => x.CategorySlug)
            .MaximumLength(150).WithMessage("Category slug cannot exceed 150 characters.");
    }
}
