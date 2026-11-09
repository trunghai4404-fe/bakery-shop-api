namespace Contents.Contracts.DTOs.Locations;

public record ProvinceDto(
    int Id,
    string Name,
    string FullName,
    string DivisionType,
    string Codename,
    int? PhoneCode
);

public record DistrictDto(
    int Id,
    int ProvinceId,
    string Name,
    string FullName,
    string DivisionType,
    string Codename
);

public record WardDto(
    int Id,
    int DistrictId,
    string Name,
    string FullName,
    string DivisionType,
    string Codename
);

public record OpenApiProvince(
    int Code,
    string Name,
    string DivisionType,
    string Codename,
    int? PhoneCode,
    List<OpenApiDistrict>? Districts
);

public record OpenApiDistrict(
    int Code,
    string Name,
    string DivisionType,
    string Codename,
    int? ProvinceCode,
    List<OpenApiWard>? Wards
);

public record OpenApiWard(
    int Code,
    string Name,
    string DivisionType,
    string Codename,
    int? DistrictCode
);
