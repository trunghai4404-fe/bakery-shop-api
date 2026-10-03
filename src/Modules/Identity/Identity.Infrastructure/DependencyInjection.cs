using System.Text;
using Identity.Application.Interfaces;
using Identity.Domain.Repository;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Repository;
using Identity.Infrastructure.Persistence.Services;
using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Domain.Interfaces;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>()
                         ?? throw new InvalidOperationException("Thiếu cấu hình 'JwtOptions'");
        services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
        
        services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthentication();
        
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());

        return services;
    }
}