using FluentValidation;

namespace Identity.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(r  => r.FullName)
            .NotEmpty().WithMessage("FullName is required")
            .MaximumLength(100).WithMessage("FullName cannot exceed 100 characters");
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");
        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must have at least 6 characters")
            .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
            .Matches(@"[A-Z]").WithMessage("Password contain at least one uppercase letter")
            .Matches(@"[0-9]").WithMessage("Password contain at least one number")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password contain at least one special character");
    }
}