using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Infrastructure.Identity;
using FER.InfSus.Time.Infrastructure.Persistence;
using FER.InfSus.Time.Infrastructure.Repositories;

namespace FER.InfSus.Time.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddTransient<ISignInService, SignInService>();
        services.AddTransient<IUserRepository, UserRepository>();
        return services;
    }
}
