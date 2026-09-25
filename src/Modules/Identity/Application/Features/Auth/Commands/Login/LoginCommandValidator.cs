using FluentValidation;

namespace Identity.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public  LoginCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
            .EmailAddress().WithMessage("Email is invalid");
        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}