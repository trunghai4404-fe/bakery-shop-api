using System.Net.Mail;
using Contents.Domain.Enums;
using Contents.Domain.Errors;
using SharedKernel.Domain;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Interfaces;

namespace Contents.Domain.Entities;

public class Contact : AuditableEntity, ISoftDelete
{
    public const int MaxValueLength = 1000;

    public ContactType Type { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    private Contact() { }

    public static Result<Contact> Create(ContactType type, string value, bool isActive)
    {
        var contact = new Contact();
        var result = contact.Update(type, value, isActive);
        return result.IsSuccess ? Result.Success(contact) : Result.Failure<Contact>(result.Error);
    }

    public Result Update(ContactType type, string value, bool isActive)
    {
        if (!Enum.IsDefined(type))
            return Result.Failure(ContentsErrors.InvalidContactType);

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure(ContentsErrors.InvalidContactValue);

        var normalizedValue = value.Trim();
        if (normalizedValue.Length > MaxValueLength || !IsValidValue(type, normalizedValue))
            return Result.Failure(ContentsErrors.InvalidContactValue);

        Type = type;
        Value = normalizedValue;
        IsActive = isActive;
        return Result.Success();
    }

    public void ToggleStatus(bool? isActive) => IsActive = isActive ?? !IsActive;

    private static bool IsValidValue(ContactType type, string value) => type switch
    {
        ContactType.Email => value.Length <= 254 && !value.Any(char.IsControl) &&
                             MailAddress.TryCreate(value, out var address) && address.Address == value,
        ContactType.Phone => IsPhone(value),
        ContactType.Zalo => IsPhone(value) || IsWebUrl(value),
        ContactType.Facebook or ContactType.TikTok => IsWebUrl(value),
        _ => false
    };

    private static bool IsPhone(string value)
    {
        var number = value.StartsWith('+') ? value[1..] : value;
        return value.Length <= 50 && number.Any(char.IsAsciiDigit) &&
               number.All(c => char.IsAsciiDigit(c) || c is ' ' or '(' or ')' or '-' or '.');
    }

    private static bool IsWebUrl(string value) =>
        !value.Any(char.IsWhiteSpace) && !value.Any(char.IsControl) &&
        Uri.TryCreate(value, UriKind.Absolute, out var url) &&
        (url.Scheme == Uri.UriSchemeHttps || url.Scheme == Uri.UriSchemeHttp) &&
        !string.IsNullOrEmpty(url.Host) && string.IsNullOrEmpty(url.UserInfo);
}
