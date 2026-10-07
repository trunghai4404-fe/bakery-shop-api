using Contents.Domain.Enums;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Interfaces;

namespace Contents.Domain.Entities;

public class Banner : AuditableEntity<Guid>, ISoftDelete
{
    public string Title { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public string? MobileImageUrl { get; private set; }
    public BannerActionType ActionType { get; private set; } = BannerActionType.None;
    public string? TargetValue { get; private set; }
    public bool OpenInNewTab { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset? StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }
    public string? Metadata { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    private Banner()
    {
    }

    private Banner(
        Guid id,
        string title,
        string imageUrl,
        string? mobileImageUrl,
        BannerActionType actionType,
        string? targetValue,
        bool openInNewTab,
        bool isActive,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        string? metadata) : base(id)
    {
        Title = title;
        ImageUrl = imageUrl;
        MobileImageUrl = mobileImageUrl;
        ActionType = actionType;
        TargetValue = targetValue;
        OpenInNewTab = openInNewTab;
        IsActive = isActive;
        StartDate = startDate;
        EndDate = endDate;
        Metadata = metadata;
    }

    public static Result<Banner> Create(
        string title,
        string imageUrl,
        string? mobileImageUrl,
        BannerActionType actionType,
        string? targetValue,
        bool openInNewTab,
        bool isActive,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        string? metadata)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Banner>(Error.Validation(ErrorCode.ValidationError, "Banner title is required."));
        }

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Result.Failure<Banner>(Error.Validation(ErrorCode.ValidationError, "Banner image URL is required."));
        }

        if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
        {
            return Result.Failure<Banner>(Error.Validation(ErrorCode.ValidationError, "End date must be greater than or equal to start date."));
        }

        if (actionType != BannerActionType.None && string.IsNullOrWhiteSpace(targetValue))
        {
            return Result.Failure<Banner>(Error.Validation(ErrorCode.ValidationError, "Target value is required when ActionType is not None."));
        }

        var banner = new Banner(
            Guid.NewGuid(),
            title.Trim(),
            imageUrl.Trim(),
            mobileImageUrl?.Trim(),
            actionType,
            targetValue?.Trim(),
            openInNewTab,
            isActive,
            startDate,
            endDate,
            metadata?.Trim());

        return Result.Success(banner);
    }

    public Result Update(
        string title,
        string imageUrl,
        string? mobileImageUrl,
        BannerActionType actionType,
        string? targetValue,
        bool openInNewTab,
        bool isActive,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate,
        string? metadata)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(Error.Validation(ErrorCode.ValidationError, "Banner title is required."));
        }

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Result.Failure(Error.Validation(ErrorCode.ValidationError, "Banner image URL is required."));
        }

        if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
        {
            return Result.Failure(Error.Validation(ErrorCode.ValidationError, "End date must be greater than or equal to start date."));
        }

        if (actionType != BannerActionType.None && string.IsNullOrWhiteSpace(targetValue))
        {
            return Result.Failure(Error.Validation(ErrorCode.ValidationError, "Target value is required when ActionType is not None."));
        }

        Title = title.Trim();
        ImageUrl = imageUrl.Trim();
        MobileImageUrl = mobileImageUrl?.Trim();
        ActionType = actionType;
        TargetValue = targetValue?.Trim();
        OpenInNewTab = openInNewTab;
        IsActive = isActive;
        StartDate = startDate;
        EndDate = endDate;
        Metadata = metadata?.Trim();

        return Result.Success();
    }

    public void ToggleStatus(bool? isActive = null)
    {
        IsActive = isActive ?? !IsActive;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }

    public BannerStatus GetStatus(DateTimeOffset? currentTime = null)
    {
        var now = currentTime ?? DateTimeOffset.UtcNow;
        if (!IsActive) return BannerStatus.Inactive;
        if (StartDate.HasValue && StartDate.Value > now) return BannerStatus.Scheduled;
        if (EndDate.HasValue && EndDate.Value < now) return BannerStatus.Expired;
        return BannerStatus.Active;
    }
}
