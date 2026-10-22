using Contents.Domain.Repositories;
using Contents.Infrastructure.Persistence;
using Contents.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
