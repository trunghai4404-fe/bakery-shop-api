namespace Catalog.Domain.Entities;

public class ProductAccessory
{
    public long ProductId { get; private set; }
    public Product? Product { get; private set; }

    public long AccessoryProductId { get; private set; }
    public Product? AccessoryProduct { get; private set; }

    public int DefaultQuantity { get; private set; } = 1;
    public bool IsIncluded { get; private set; } = true;
    public int SortOrder { get; private set; }
    public bool IsMultiSelect { get; private set; } = false;
    public int? MaxSelectCount { get; private set; }
    public bool IsRequired { get; private set; } = false;

    private ProductAccessory() { }

    public ProductAccessory(
        long productId,
        long accessoryProductId,
        int defaultQuantity = 1,
        bool isIncluded = true,
        int sortOrder = 0,
        bool isMultiSelect = false,
        int? maxSelectCount = null,
        bool isRequired = false)
    {
        ProductId = productId;
        AccessoryProductId = accessoryProductId;
        DefaultQuantity = defaultQuantity > 0 ? defaultQuantity : 1;
        IsIncluded = isIncluded;
        SortOrder = sortOrder;
        IsMultiSelect = isMultiSelect;
        MaxSelectCount = maxSelectCount;
        IsRequired = isRequired;
    }

    public void Update(
        int defaultQuantity,
        bool isIncluded,
        int sortOrder,
        bool isMultiSelect = false,
        int? maxSelectCount = null,
        bool isRequired = false)
    {
        DefaultQuantity = defaultQuantity > 0 ? defaultQuantity : 1;
        IsIncluded = isIncluded;
        SortOrder = sortOrder;
        IsMultiSelect = isMultiSelect;
        MaxSelectCount = maxSelectCount;
        IsRequired = isRequired;
    }
}
