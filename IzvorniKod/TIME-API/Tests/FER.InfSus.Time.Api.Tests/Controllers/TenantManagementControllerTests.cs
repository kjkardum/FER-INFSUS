using AutoBogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FER.InfSus.Time.Api.Controllers;
using FER.InfSus.Time.Api.Services;
using FER.InfSus.Time.Application.Request;
using FER.InfSus.Time.Application.Response;
using FER.InfSus.Time.Application.UseCases.User.Commands.Create;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Application.UseCases.User.Commands.Update;
using FER.InfSus.Time.Application.UseCases.User.Queries.GetPaginated;
using Xunit;

namespace FER.InfSus.Time.Api.Tests.Controllers;

public class TenantManagementControllerTests
{
    private readonly TenantManagementController _controller;
    private readonly IMediator _mediator;
    private readonly IAuthenticationService _authenticationService;

    public TenantManagementControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _authenticationService = Substitute.For<IAuthenticationService>();
        _controller = new TenantManagementController(_mediator, _authenticationService);
    }

    [Fact]
    public async Task CreateUser_Returns_Ok()
    {
        // Arrange
        var dto = AutoFaker.Generate<UserRegistrationCommand>();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.CreateUser(dto, cancellationToken);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task GetPaginatedUsers_Returns_PaginatedResponseOfUserDto()
    {
        // Arrange
        var request = AutoFaker.Generate<PaginatedRequest>();
        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<PaginatedResponse<UserDto>>();

        _authenticationService.GetUserId().Returns(Guid.NewGuid());
        _mediator.Send(Arg.Any<UserGetPaginatedQuery>(), cancellationToken).Returns(response);

        // Act
        var result = await _controller.GetPaginatedUsers(request, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        var controllerMethodReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.GetPaginatedUsers))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0]; // Type
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType(controllerMethodReturnType);
    }

    [Fact]
    public async Task UpdateUser_Returns_Ok()
    {
        // Arrange
        var dto = AutoFaker.Generate<UserUpdateCommand>();
        var id = Guid.NewGuid();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.UpdateUser(id, dto, cancellationToken);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task DeleteUser_Returns_Ok()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.DeleteUser(id, cancellationToken);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}
