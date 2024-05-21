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

public class AuthenticationControllerTests: WebHostTestBase
{
    private readonly AuthenticationController _controller;
    private readonly IMediator _mediator;

    public AuthenticationControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new AuthenticationController(_mediator);
    }

    [Fact]
    public async Task Login_User_Returns_User()
    {
        //Arrange
        var dto = AutoFaker.Generate<UserLoginCommand>();

        var cancellationToken = default(CancellationToken);
        var request = Arg.Is<UserLoginCommand>(
            x => x.Email == dto.Email);
        var response = AutoFaker.Generate<LoggedInUserDto>();

        _mediator.Send(request, cancellationToken).Returns(response);

        //Act
        var result = await _controller.Login(
            dto,
            cancellationToken);

        //Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        responseResult.Should().NotBeNull();
        responseResult!.Value.Should().BeEquivalentTo(response);
    }
}
