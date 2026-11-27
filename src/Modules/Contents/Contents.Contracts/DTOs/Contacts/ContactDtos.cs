namespace Contents.Contracts.DTOs.Contacts;

public record ContactRequest(int Type, string Value, bool IsActive = true);

public record ContactDto(
    Guid Id,
    string Type,
    int TypeValue,
    string Value,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastModifiedAt
);

public record ActiveContactDto(Guid Id, string Type, int TypeValue, string Value);

public record ToggleContactRequest(bool? IsActive = null);
public record ToggleContactResponse(Guid Id, bool IsActive);
