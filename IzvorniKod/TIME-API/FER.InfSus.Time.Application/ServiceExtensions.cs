using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FER.InfSus.Time.Application.Behaviours;
using FER.InfSus.Time.Application.Configuration;

namespace FER.InfSus.Time.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = typeof(IExplicitAssemblyReference).Assembly;
        services.AddAutoMapper(assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssemblies( new[] { assembly });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfiguration.ConfigKey));
        services.Configure<AppConfiguration>(configuration.GetSection(AppConfiguration.ConfigKey));

        return services;
    }
}
