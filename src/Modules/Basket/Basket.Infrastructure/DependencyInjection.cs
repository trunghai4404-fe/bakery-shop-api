using Basket.Domain.Repositories;
using Basket.Infrastructure.Persistence;
using Basket.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBasketInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<BasketDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IBasketUnitOfWork>(sp => sp.GetRequiredService<BasketDbContext>());
        services.AddScoped<IBasketRepository, BasketRepository>();

        return services;
    }
}
