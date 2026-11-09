using SharedKernel.Domain.Entities;

namespace Contents.Domain.Entities;

public class Ward : BaseEntity<int>
{
    public int DistrictId { get; private set; }
    public District District { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string DivisionType { get; private set; } = string.Empty;
    public string Codename { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; } = 0;

    private Ward() { }

    public Ward(
        int id, 
        int districtId, 
        string name, 
        string fullName, 
        string divisionType, 
        string codename, 
        bool isActive = true, 
        int sortOrder = 0)
        : base(id)
    {
        DistrictId = districtId;
        Name = name;
        FullName = fullName;
        DivisionType = divisionType;
        Codename = codename;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public void Update(string name, string fullName, string divisionType, string codename, bool isActive = true)
    {
        Name = name;
        FullName = fullName;
        DivisionType = divisionType;
        Codename = codename;
        IsActive = isActive;
    }
}
