namespace SharedKernel.Exceptions;

public class ValidationException(IReadOnlyCollection<ValidationErrorDetail> errors)
    : Exception("One or more validation errors occurred.")
{
    public IReadOnlyCollection<ValidationErrorDetail> Errors { get; } = errors;
}

public record ValidationErrorDetail(string PropertyName, string ErrorMessage);
