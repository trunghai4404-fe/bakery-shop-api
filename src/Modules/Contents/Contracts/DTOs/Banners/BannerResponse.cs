namespace Contents.Contracts.DTOs.Banners;

public class BannerResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? MobileImageUrl { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public int ActionTypeValue { get; set; }
    public string? TargetValue { get; set; }
    public bool OpenInNewTab { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StatusValue { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
