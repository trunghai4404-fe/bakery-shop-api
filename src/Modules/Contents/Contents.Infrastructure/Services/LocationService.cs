using System.Globalization;
using System.Text;
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

    public async Task<List<ProvinceDto>> GetProvincesAsync(string? keyword = null, CancellationToken ct = default)
    {
        const string cacheKey = "locations:provinces";
        List<ProvinceDto>? dtos = null;

        try
        {
            var cached = await cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedProvinces = JsonSerializer.Deserialize<List<ProvinceDto>>(cached, JsonOptions);
                if (cachedProvinces != null && cachedProvinces.Count > 0)
                {
                    dtos = cachedProvinces;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read provinces from Redis cache. Falling back to DB.");
        }

        if (dtos == null)
        {
            var provinces = await locationRepository.GetProvincesAsync(ct: ct);

            if (provinces.Count == 0)
            {
                logger.LogInformation("Database chưa có dữ liệu địa giới hành chính. Bắt đầu tự động đồng bộ...");
                await SyncLocationsFromOpenApiAsync(ct);
                provinces = await locationRepository.GetProvincesAsync(ct: ct);
            }

            dtos = provinces.Select(p => new ProvinceDto(
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
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            return dtos.Where(p => MatchesKeyword(p.Name, p.FullName, p.Codename, keyword)).ToList();
        }

        return dtos;
    }

    public async Task<List<DistrictDto>> GetDistrictsAsync(int? provinceId = null, string? keyword = null, CancellationToken ct = default)
    {
        var cacheKey = provinceId.HasValue 
            ? $"locations:districts:{provinceId.Value}" 
            : "locations:districts:all";

        List<DistrictDto>? dtos = null;

        try
        {
            var cached = await cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedDistricts = JsonSerializer.Deserialize<List<DistrictDto>>(cached, JsonOptions);
                if (cachedDistricts != null)
                {
                    dtos = cachedDistricts;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read districts from Redis cache (key: {CacheKey}). Falling back to DB.", cacheKey);
        }

        if (dtos == null)
        {
            var districts = await locationRepository.GetDistrictsAsync(provinceId, ct: ct);
            dtos = districts.Select(d => new DistrictDto(
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
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            return dtos.Where(d => MatchesKeyword(d.Name, d.FullName, d.Codename, keyword)).ToList();
        }

        return dtos;
    }

    public async Task<List<WardDto>> GetWardsAsync(int? districtId = null, int? provinceId = null, string? keyword = null, CancellationToken ct = default)
    {
        var cacheKey = districtId.HasValue 
            ? $"locations:wards:district:{districtId.Value}" 
            : (provinceId.HasValue ? $"locations:wards:province:{provinceId.Value}" : "locations:wards:all");

        List<WardDto>? dtos = null;

        try
        {
            var cached = await cache.GetStringAsync(cacheKey, ct);
            if (!string.IsNullOrEmpty(cached))
            {
                var cachedWards = JsonSerializer.Deserialize<List<WardDto>>(cached, JsonOptions);
                if (cachedWards != null)
                {
                    dtos = cachedWards;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read wards from Redis cache (key: {CacheKey}). Falling back to DB.", cacheKey);
        }

        if (dtos == null)
        {
            var wards = await locationRepository.GetWardsAsync(districtId, provinceId, ct: ct);
            dtos = wards.Select(w => new WardDto(
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
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            return dtos.Where(w => MatchesKeyword(w.Name, w.FullName, w.Codename, keyword)).ToList();
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

    private static bool MatchesKeyword(string name, string fullName, string codename, string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return true;
        var kw = keyword.Trim();
        var kwNormalized = RemoveDiacritics(kw);

        if (name.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
            fullName.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
            codename.Contains(kw, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var nameNormalized = RemoveDiacritics(name);
        var fullNameNormalized = RemoveDiacritics(fullName);

        return nameNormalized.Contains(kwNormalized, StringComparison.OrdinalIgnoreCase) ||
               fullNameNormalized.Contains(kwNormalized, StringComparison.OrdinalIgnoreCase) ||
               codename.Replace('_', ' ').Contains(kwNormalized, StringComparison.OrdinalIgnoreCase);
    }

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
    }
}
