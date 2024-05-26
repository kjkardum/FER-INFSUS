using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Infrastructure.Identity;
using FER.InfSus.Time.Infrastructure.Persistence;
using FER.InfSus.Time.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;

namespace FER.InfSus.Time.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (connectionString != "TestConnectionString")
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                options.UseSqlite(new SqliteConnection("DataSource=file::memory:?cache=shared"));
            }
        });

        services.AddTransient<ISignInService, SignInService>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<ITaskboardRepository, TaskboardRepository>();
        services.AddTransient<ITaskItemRepository, TaskItemRepository>();
        return services;
    }
}
