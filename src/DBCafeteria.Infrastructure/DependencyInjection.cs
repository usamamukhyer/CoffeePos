using DBCafeteria.Application.Interfaces;
using DBCafeteria.Infrastructure.Data;
using DBCafeteria.Infrastructure.Security;
using DBCafeteria.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DBCafeteria.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CafeDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CafeDatabase")));

        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IGiftService, GiftService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
