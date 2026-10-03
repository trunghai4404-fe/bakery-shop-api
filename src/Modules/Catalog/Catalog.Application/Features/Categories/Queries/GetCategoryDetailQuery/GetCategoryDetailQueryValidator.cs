using FluentValidation;

namespace Catalog.Application.Features.Categories.Queries.GetCategoryDetailQuery;

public class GetCategoryDetailQueryValidator : AbstractValidator<GetCategoryDetailQuery>
{
    public GetCategoryDetailQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Category ID must be greater than 0.");
    }
}
