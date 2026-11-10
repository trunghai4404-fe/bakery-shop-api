using Microsoft.Extensions.DependencyInjection;

namespace Basket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBasketApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        return services;
    }
}
