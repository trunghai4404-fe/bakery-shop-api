using SharedKernel.Domain.Entities;

namespace Catalog.Domain.Entities;

public class ProductImage : AuditableEntity<long>
{
    public long ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public string DesktopUrl { get; private set; } = string.Empty;
    public string MobileUrl { get; private set; }  = string.Empty;
    public bool IsPrimary { get; private set; }
    public int SortOrder { get; private set; }

    private ProductImage()
    {
        
    }

    internal ProductImage(long productId, string desktopUrl, string mobileUrl,  int sortOrder, bool isPrimary)
    {
        ProductId = productId;
        DesktopUrl = desktopUrl;
        MobileUrl = mobileUrl;
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
    }

    internal void UpdateImage(string desktopUrl, string mobileUrl, int sortOrder, bool isPrimary)
    {
        DesktopUrl = desktopUrl;
        MobileUrl = mobileUrl;
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
    }
}