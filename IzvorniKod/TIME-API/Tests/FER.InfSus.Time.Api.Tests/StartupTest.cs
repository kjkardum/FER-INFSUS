using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Swashbuckle.AspNetCore.Swagger;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Infrastructure.Persistence;
using Xunit;

namespace FER.InfSus.Time.Api.Tests;

public class StartupTest : WebHostTestBase
{
    private IConfiguration? _configuration;
    private IWebHostEnvironment? _webHostEnvironment;

    [Fact]
    public void ConfigureServices_HasRegisteredAllServices_Production()
    {
        //Arrange
        _configuration = MockDefaultConfiguration();
        _webHostEnvironment = Substitute.For<IWebHostEnvironment>();

        var (services, controllers) = PrepareControllerTest(_webHostEnvironment);

        //Act
        Program.ConfigureServices(services, _configuration, _webHostEnvironment);

        //Assert
        var serviceProvider = services.BuildServiceProvider();
        AssertDefaultRegisteredServices(serviceProvider, controllers);
    }

    private static void AssertDefaultRegisteredServices(
        IServiceProvider serviceProvider,
        IEnumerable<Type> typesToCheck)
    {
        AssertRegisteredService<ApplicationDbContext>(serviceProvider);
        AssertRegisteredService<IUserRepository>(serviceProvider);
        AssertRegisteredService<IMapper>(serviceProvider);
        AssertRegisteredService<IHttpContextAccessor>(serviceProvider);

        foreach (var type in typesToCheck)
        {
            AssertRegisteredService(serviceProvider, type);
        }
    }

    private static void AssertRegisteredService<T>(IServiceProvider serviceProvider)
        => AssertRegisteredService(serviceProvider, typeof(T));

    private static void AssertRegisteredService(IServiceProvider serviceProvider, Type type)
    {
        var resolvedType = serviceProvider.GetService(type);
        resolvedType.Should().NotBeNull();
    }
}
