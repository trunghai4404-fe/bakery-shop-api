using FluentValidation;

namespace Identity.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public  UpdateProfileCommandValidator()
    {
        When(c => c.FullName != null, () =>
        {
            RuleFor(c => c.FullName)
                .NotEmpty().WithMessage("The full name is required")
                .MaximumLength(100).WithMessage("The full name cannot exceed 100 characters");
        });
        When(c => c.PhoneNumber != null, () =>
        {
            RuleFor(c => c.PhoneNumber)
                .NotEmpty().WithMessage("The phone number is required")
                .Matches(@"^\+?[0-9]{9,15}$").WithMessage("The phone number is invalid");
        });
        When(c => c.DateOfBirth != null, () =>
        {
            RuleFor(c => c.DateOfBirth)
                .LessThan(DateTime.UtcNow).WithMessage("The date of birth must be in the past");
        });
    }
}