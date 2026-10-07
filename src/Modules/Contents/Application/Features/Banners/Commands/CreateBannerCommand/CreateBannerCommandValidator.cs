using Contents.Domain.Enums;
using FluentValidation;

namespace Contents.Application.Features.Banners.Commands.CreateBannerCommand;

public class CreateBannerCommandValidator : AbstractValidator<CreateBannerCommand>
{
    public CreateBannerCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Banner title is required.")
            .MaximumLength(250).WithMessage("Banner title cannot exceed 250 characters.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Banner image URL is required.")
            .MaximumLength(1000).WithMessage("Image URL cannot exceed 1000 characters.");

        RuleFor(x => x.MobileImageUrl)
            .MaximumLength(1000).WithMessage("Mobile image URL cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.MobileImageUrl));

        RuleFor(x => x.TargetValue)
            .NotEmpty().WithMessage("Target value is required when ActionType is not None.")
            .When(x => x.ActionType != BannerActionType.None);

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate!.Value)
            .WithMessage("End date must be greater than or equal to start date.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
