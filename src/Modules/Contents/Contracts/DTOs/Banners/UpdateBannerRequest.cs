namespace Contents.Contracts.DTOs.Banners;

public class UpdateBannerRequest
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? MobileImageUrl { get; set; }
    public int ActionType { get; set; }
    public string? TargetValue { get; set; }
    public bool OpenInNewTab { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? Metadata { get; set; }
}
