using Contents.Domain.Entities;
using Contents.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Contents.Infrastructure.Persistence.Repositories;

public class LocationRepository(ContentsDbContext dbContext) : ILocationRepository
{
    public async Task<List<Province>> GetProvincesAsync(CancellationToken ct = default)
    {
        return await dbContext.Provinces
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Id)
            .ToListAsync(ct);
    }

    public async Task<List<District>> GetDistrictsAsync(int? provinceId = null, CancellationToken ct = default)
    {
        var query = dbContext.Districts
            .AsNoTracking()
            .Where(d => d.IsActive);

        if (provinceId.HasValue)
        {
            query = query.Where(d => d.ProvinceId == provinceId.Value);
        }

        return await query
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.Id)
            .ToListAsync(ct);
    }

    public async Task<List<Ward>> GetWardsAsync(int? districtId = null, int? provinceId = null, CancellationToken ct = default)
    {
        var query = dbContext.Wards
            .AsNoTracking()
            .Where(w => w.IsActive);

        if (districtId.HasValue)
        {
            query = query.Where(w => w.DistrictId == districtId.Value);
        }
        else if (provinceId.HasValue)
        {
            query = query.Where(w => w.District.ProvinceId == provinceId.Value);
        }

        return await query
            .OrderBy(w => w.SortOrder)
            .ThenBy(w => w.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> HasLocationsAsync(CancellationToken ct = default)
    {
        return await dbContext.Provinces.AnyAsync(ct);
    }

    public async Task UpsertLocationsAsync(List<Province> provinces, CancellationToken ct = default)
    {
        // Sử dụng ExecutionStrategy và Transaction để đảm bảo tính toàn vẹn
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            // Kiểm tra xem đã có dữ liệu chưa
            var existingProvinces = await dbContext.Provinces
                .Include(p => p.Districts)
                .ThenInclude(d => d.Wards)
                .ToDictionaryAsync(p => p.Id, ct);

            foreach (var province in provinces)
            {
                if (!existingProvinces.TryGetValue(province.Id, out var existingProvince))
                {
                    await dbContext.Provinces.AddAsync(province, ct);
                }
                else
                {
                    existingProvince.Update(
                        province.Name, 
                        province.FullName, 
                        province.DivisionType, 
                        province.Codename, 
                        province.PhoneCode, 
                        province.IsActive);

                    var existingDistricts = existingProvince.Districts.ToDictionary(d => d.Id);

                    foreach (var district in province.Districts)
                    {
                        if (!existingDistricts.TryGetValue(district.Id, out var existingDistrict))
                        {
                            existingProvince.Districts.Add(district);
                        }
                        else
                        {
                            existingDistrict.Update(
                                district.Name, 
                                district.FullName, 
                                district.DivisionType, 
                                district.Codename, 
                                district.IsActive);

                            var existingWards = existingDistrict.Wards.ToDictionary(w => w.Id);

                            foreach (var ward in district.Wards)
                            {
                                if (!existingWards.TryGetValue(ward.Id, out var existingWard))
                                {
                                    existingDistrict.Wards.Add(ward);
                                }
                                else
                                {
                                    existingWard.Update(
                                        ward.Name, 
                                        ward.FullName, 
                                        ward.DivisionType, 
                                        ward.Codename, 
                                        ward.IsActive);
                                }
                            }
                        }
                    }
                }
            }

            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        });
    }
}
