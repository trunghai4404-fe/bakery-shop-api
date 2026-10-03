using FluentValidation;

namespace Catalog.Application.Features.Categories.Queries.GetAllCategoriesQuery;

public class GetAllCategoriesQueryValidator : AbstractValidator<GetAllCategoriesQuery>
{
    public GetAllCategoriesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize must be greater than or equal to 1.")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100.");

        RuleFor(x => x.ParentId)
            .GreaterThan(0).When(x => x.ParentId.HasValue)
            .WithMessage("ParentId must be greater than 0.");

        RuleFor(x => x.Search)
            .MaximumLength(100).WithMessage("Search keyword cannot exceed 100 characters.");
    }
}
