using FluentValidation;

namespace Identity.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public  CreateRoleCommandValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        RuleFor(r => r.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(100).WithMessage("Code must not exceed 100 characters");
    }
}