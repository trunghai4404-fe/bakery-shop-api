using SharedKernel.Domain.Entities;

namespace Contents.Domain.Entities;

public class Province : BaseEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string DivisionType { get; private set; } = string.Empty;
    public string Codename { get; private set; } = string.Empty;
    public int? PhoneCode { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; } = 0;

    public ICollection<District> Districts { get; private set; } = new List<District>();

    private Province() { }

    public Province(int id, string name, string fullName, string divisionType, string codename, int? phoneCode, bool isActive = true, int sortOrder = 0)
        : base(id)
    {
        Name = name;
        FullName = fullName;
        DivisionType = divisionType;
        Codename = codename;
        PhoneCode = phoneCode;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public void Update(string name, string fullName, string divisionType, string codename, int? phoneCode, bool isActive = true)
    {
        Name = name;
        FullName = fullName;
        DivisionType = divisionType;
        Codename = codename;
        PhoneCode = phoneCode;
        IsActive = isActive;
    }
}
