using Contents.Contracts.Interfaces;
using Contents.Domain.Repositories;
using Contents.Infrastructure.Persistence;
using Contents.Infrastructure.Persistence.Repositories;
using Contents.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Contents.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddContentsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<ContentsDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IContentsUnitOfWork>(sp => sp.GetRequiredService<ContentsDbContext>());
        services.AddScoped<IBannerRepository, BannerRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ILocationService, LocationService>();

        // Cấu hình Redis Cache (Distributed Cache)
        var redisConn = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConn))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                var configOptions = ConfigurationOptions.Parse(redisConn);
                configOptions.ConnectTimeout = 1000; // Timeout 1s thay vì 5s mặc định
                configOptions.SyncTimeout = 1000;
                configOptions.AbortOnConnectFail = false; // Không crash ứng dụng nếu Redis chưa bật

                options.ConfigurationOptions = configOptions;
                options.InstanceName = "BakeryShop_";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        // Cấu hình HttpClient để lấy dữ liệu từ provinces.open-api.vn
        services.AddHttpClient("ProvincesApiClient", client =>
        {
            var baseUrl = configuration["ProvincesApi:BaseUrl"] ?? "https://provinces.open-api.vn/api/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        return services;
    }
}
