using AutoBogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FER.InfSus.Time.Api.Controllers;
using FER.InfSus.Time.Api.Services;
using FER.InfSus.Time.Application.UseCases.User.Commands.Create;
using FER.InfSus.Time.Application.UseCases.User.Commands.Login;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using Xunit;

namespace FER.InfSus.Time.Api.Tests.Controllers;

public class AuthenticationControllerTests
{
    private readonly AuthenticationController _controller;
    private readonly IMediator _mediator;

    public AuthenticationControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new AuthenticationController(_mediator);
    }

    [Fact]
    public async Task Login_User_Returns_LoggedInUserDto()
    {
        //Arrange
        var dto = AutoFaker.Generate<UserLoginCommand>();

        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<LoggedInUserDto>();

        _mediator.Send(
            Arg.Is<UserLoginCommand>(
                x => x.Email == dto.Email),
            cancellationToken)
            .Returns(response);

        //Act
        var result = await _controller.Login(
            dto,
            cancellationToken);

        //Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        // Controller method ActionResult generic return type doesn't ensure the type of actual value returned if used with OkObjectResult
        var controllerMethodReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.Login))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0]; // Type
        responseResult.Should().NotBeNull();
        responseResult!.Value.Should().NotBeNull();
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType(controllerMethodReturnType);
    }
}
