using FluentValidation;

namespace Identity.Application.Features.Roles.Commands.UpdateRole;

public class Validator : AbstractValidator<UpdateRoleCommand>
{
    public Validator()
    {
        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        RuleFor(ur => ur.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(100).WithMessage("Description must not exceed 100 characters");
    }
}