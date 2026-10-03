using FluentValidation;

namespace Identity.Application.Features.Auth.Commands.Logout;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(c => c.RefreshToken)
            .NotEmpty().WithMessage("The refresh token is required");
    }
}