using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace FER.InfSus.Time.Api.Tests;

public abstract class WebHostTestBase
{

    private readonly IConfiguration? _configuration;
    private readonly IWebHostEnvironment? _webHostEnvironment;

    protected ServiceProvider ServiceProvider;

    protected WebHostTestBase()
    {
        _configuration = MockDefaultConfiguration();
        _webHostEnvironment = Substitute.For<IWebHostEnvironment>();

        (var services, var _) = PrepareControllerTest(_webHostEnvironment);

        //Act
        Program.ConfigureServices(services, _configuration, _webHostEnvironment);

        //Assert
        ServiceProvider = services.BuildServiceProvider();
    }

    protected static (ServiceCollection services, IEnumerable<Type> controllers) RegisterControllers(
        ServiceCollection services)
    {
        var assembly = typeof(Program).Assembly;
        var controllers = assembly
            .GetTypes()
            .Where(type =>
                typeof(ControllerBase).IsAssignableFrom(type));

        foreach (var controller in controllers)
        {
            services.AddScoped(controller);
        }

        return (services, controllers);
    }

    protected static (ServiceCollection services, IEnumerable<Type> controllers) PrepareControllerTest(
        IWebHostEnvironment webHostEnvironment)
    {
        (var services, var controllers) = RegisterControllers(new ServiceCollection());
        services.AddTransient(_ => webHostEnvironment);
        //needed for application insights, otherwise some GetService calls will throw an exception
#pragma warning disable CS0618 // Type or member is obsolete
        var hostingEnvironment = Substitute.For<IHostingEnvironment>();
#pragma warning restore CS0618 // Type or member is obsolete
        hostingEnvironment.ContentRootPath.Returns("/");
        hostingEnvironment.WebRootPath.Returns("/");
        services.AddTransient(_ => hostingEnvironment);

        return (services, controllers);
    }

    protected static IConfiguration MockDefaultConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ASPNETCORE_URLS", "https://localhost:5000;http://localhost:6000" },
            { "ConnectionStrings:DefaultConnection", "TestConnectionString" },
        };

        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.AddInMemoryCollection(inMemorySettings).Build();

        return configuration;
    }
}
