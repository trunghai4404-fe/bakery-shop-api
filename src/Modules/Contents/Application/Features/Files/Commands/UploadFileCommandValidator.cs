using Contents.Application.Features.Files.Commands;
using FluentValidation;

namespace Contents.Application.Features.Files.Commands;

public class UploadFileCommandValidator : AbstractValidator<UploadFilesCommand>
{
    private readonly string[] _permittedExtensions = { ".jpg", ".jpeg", ".png" };
    
    public UploadFileCommandValidator()
    {
        RuleFor(x => x.Files)
            .NotNull().WithMessage("Files can't be null")
            .NotEmpty().WithMessage("Files can't be empty");
        RuleForEach(x => x.Files).ChildRules(file => 
        {
            file.RuleFor(f => f.FileName)
                .NotEmpty().WithMessage("File name can't be empty")
                .Must(HaveValidExtension)
                .WithMessage("The file only accepts the following formats: .png .jpg .jpeg");
            file.RuleFor(f => f.ContentType)
                .NotEmpty().WithMessage("File type can't be empty")
                .Must(c => c.StartsWith("image/"))
                .WithMessage("File type  can't start with 'image/'");
            file.RuleFor(f => f.Stream.Length)
                .LessThanOrEqualTo(10 * 1024 * 1024)
                .WithMessage("The file size must be greater than or equal to 10 MB");
        });
    }
    private bool HaveValidExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) 
            return false;
            
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return _permittedExtensions.Contains(extension);
    }
}