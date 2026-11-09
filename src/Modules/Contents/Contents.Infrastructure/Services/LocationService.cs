using System.Text.Json;
using Contents.Contracts.DTOs.Locations;
using Contents.Contracts.Interfaces;
using Contents.Domain.Entities;
using Contents.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contents.Infrastructure.Services;

public class LocationService(
    ILocationRepository locationRepository,
    IDistributedCache cache,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<LocationService> logger) : ILocationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly TimeSpan _cacheDuration = TimeSpan.FromDays(
        configuration.GetValue<int>("ProvincesApi:CacheDurationDays", 30));

    public async Task<List<ProvinceDto>> GetProvincesAsync(CancellationToken ct = default)
    {
        const string cacheKey = "locations:provinces";

        try
        {
            var cached = await cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedProvinces = JsonSerializer.Deserialize<List<ProvinceDto>>(cached, JsonOptions);
                if (cachedProvinces != null && cachedProvinces.Count > 0)
                {
                    return cachedProvinces;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read provinces from Redis cache. Falling back to DB.");
        }

        var provinces = await locationRepository.GetProvincesAsync(ct);

        if (provinces.Count == 0)
        {
            logger.LogInformation("Database chưa có dữ liệu địa giới hành chính. Bắt đầu tự động đồng bộ...");
            await SyncLocationsFromOpenApiAsync(ct);
            provinces = await locationRepository.GetProvincesAsync(ct);
        }

        var dtos = provinces.Select(p => new ProvinceDto(
            p.Id,
            p.Name,
            p.FullName,
            p.DivisionType,
            p.Codename,
            p.PhoneCode
        )).ToList();

        try
        {
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos, JsonOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save provinces to Redis cache.");
        }

        return dtos;
    }

    public async Task<List<DistrictDto>> GetDistrictsAsync(int? provinceId = null, CancellationToken ct = default)
    {
        var cacheKey = provinceId.HasValue 
            ? $"locations:districts:{provinceId.Value}" 
            : "locations:districts:all";

        try
        {
            var cached = await cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedDistricts = JsonSerializer.Deserialize<List<DistrictDto>>(cached, JsonOptions);
                if (cachedDistricts != null)
                {
                    return cachedDistricts;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read districts from Redis cache (key: {CacheKey}). Falling back to DB.", cacheKey);
        }

        var districts = await locationRepository.GetDistrictsAsync(provinceId, ct);
        var dtos = districts.Select(d => new DistrictDto(
            d.Id,
            d.ProvinceId,
            d.Name,
            d.FullName,
            d.DivisionType,
            d.Codename
        )).ToList();

        try
        {
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos, JsonOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save districts to Redis cache (key: {CacheKey}).", cacheKey);
        }

        return dtos;
    }

    public async Task<List<WardDto>> GetWardsAsync(int? districtId = null, int? provinceId = null, CancellationToken ct = default)
    {
        var cacheKey = districtId.HasValue 
            ? $"locations:wards:district:{districtId.Value}" 
            : (provinceId.HasValue ? $"locations:wards:province:{provinceId.Value}" : "locations:wards:all");

        try
        {
            var cached = await cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedWards = JsonSerializer.Deserialize<List<WardDto>>(cached, JsonOptions);
                if (cachedWards != null)
                {
                    return cachedWards;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read wards from Redis cache (key: {CacheKey}). Falling back to DB.", cacheKey);
        }

        var wards = await locationRepository.GetWardsAsync(districtId, provinceId, ct);
        var dtos = wards.Select(w => new WardDto(
            w.Id,
            w.DistrictId,
            w.Name,
            w.FullName,
            w.DivisionType,
            w.Codename
        )).ToList();

        try
        {
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dtos, JsonOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to save wards to Redis cache (key: {CacheKey}).", cacheKey);
        }

        return dtos;
    }

    public async Task<int> SyncLocationsFromOpenApiAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Bắt đầu đồng bộ danh mục hành chính từ provinces.open-api.vn...");

        var client = httpClientFactory.CreateClient("ProvincesApiClient");
        var response = await client.GetAsync("?depth=3", ct);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var openApiProvinces = await JsonSerializer.DeserializeAsync<List<OpenApiProvince>>(stream, JsonOptions, ct) ?? [];

        if (openApiProvinces.Count == 0)
        {
            logger.LogWarning("Không nhận được dữ liệu từ provinces.open-api.vn");
            return 0;
        }

        var provinces = new List<Province>();

        foreach (var p in openApiProvinces)
        {
            var province = new Province(
                p.Code,
                p.Name,
                p.Name,
                p.DivisionType ?? string.Empty,
                p.Codename ?? string.Empty,
                p.PhoneCode,
                true);

            if (p.Districts != null)
            {
                foreach (var d in p.Districts)
                {
                    var district = new District(
                        d.Code,
                        p.Code,
                        d.Name,
                        d.Name,
                        d.DivisionType ?? string.Empty,
                        d.Codename ?? string.Empty,
                        true,
                        0);

                    if (d.Wards != null)
                    {
                        foreach (var w in d.Wards)
                        {
                            var ward = new Ward(
                                w.Code,
                                d.Code,
                                w.Name,
                                w.Name,
                                w.DivisionType ?? string.Empty,
                                w.Codename ?? string.Empty,
                                true,
                                0);

                            district.Wards.Add(ward);
                        }
                    }

                    province.Districts.Add(district);
                }
            }

            provinces.Add(province);
        }

        await locationRepository.UpsertLocationsAsync(provinces, ct);
        logger.LogInformation("Đã lưu {Count} tỉnh/thành vào PostgreSQL.", provinces.Count);

        try
        {
            var dtos = provinces.Select(p => new ProvinceDto(
                p.Id,
                p.Name,
                p.FullName,
                p.DivisionType,
                p.Codename,
                p.PhoneCode
            )).ToList();

            await cache.SetStringAsync("locations:provinces", JsonSerializer.Serialize(dtos, JsonOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Lỗi khi warm-up Redis cache sau khi sync.");
        }

        return provinces.Count;
    }
}
