using AutoBogus;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using FER.InfSus.Time.Api.Controllers;
using FER.InfSus.Time.Api.Services;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.AddUser;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Create;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Delete;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.RemoveUser;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Update;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByTenant;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByUserId;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetById;
using System.Collections;
using Xunit;

namespace FER.InfSus.Time.Api.Tests.Controllers;

public class TaskboardControllerTests : WebHostTestBase
{
    private readonly TaskboardController _controller;
    private readonly IMediator _mediator;
    private readonly IAuthenticationService _authenticationService;

    public TaskboardControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _authenticationService = Substitute.For<IAuthenticationService>();
        _controller = new TaskboardController(_mediator, _authenticationService);
    }

    [Fact]
    public async Task GetTaskboard_Returns_TaskboardDetailedDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<TaskboardDetailedDto>();

        _authenticationService.GetUserId().Returns(Guid.NewGuid());
        _mediator.Send(Arg.Any<TaskboardGetByIdQuery>(), cancellationToken).Returns(response);

        // Act
        var result = await _controller.GetTaskboard(id, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        var controllerMethodReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.GetTaskboard))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0]; // Type
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType(controllerMethodReturnType);
    }

    [Fact]
    public async Task GetAssignedTaskboards_Returns_ICollectionOfTaskboardSimpleDto()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<TaskboardSimpleDto>(5);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());
        _mediator.Send(Arg.Any<TaskboardGetAllByUserIdQuery>(), cancellationToken).Returns(response);

        // Act
        var result = await _controller.GetAssignedTaskboards(cancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        var controllerMethodSingleItemReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.GetAssignedTaskboards))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0] // Type (ICollection)
            .GetGenericArguments()[0]; // Type (TaskboardSimpleDto)
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType<List<TaskboardSimpleDto>>();
        (responseResult!.Value as IList)![0].Should().BeOfType(controllerMethodSingleItemReturnType);
    }

    [Fact]
    public async Task GetAllTaskboards_Returns_ICollectionOfTaskboardSimpleDto()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<TaskboardSimpleDto>(5);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());
        _mediator.Send(Arg.Any<TaskboardGetAllByTenantQuery>(), cancellationToken).Returns(response);

        // Act
        var result = await _controller.GetAllTaskboards(cancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        var controllerMethodSingleItemReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.GetAllTaskboards))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0] // Type (ICollection)
            .GetGenericArguments()[0]; // Type (TaskboardSimpleDto)
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType<List<TaskboardSimpleDto>>();
        (responseResult!.Value as IList)![0].Should().BeOfType(controllerMethodSingleItemReturnType);
    }

    [Fact]
    public async Task CreateTaskboard_Returns_CreatedAtActionResult()
    {
        // Arrange
        var dto = AutoFaker.Generate<TaskboardCreateCommand>();
        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<TaskboardSimpleDto>();

        _authenticationService.GetUserId().Returns(Guid.NewGuid());
        _mediator.Send(Arg.Any<TaskboardCreateCommand>(), cancellationToken).Returns(response);

        // Act
        var result = await _controller.CreateTaskboard(dto, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var responseResult = result.Result as CreatedAtActionResult;
        var controllerMethodReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.CreateTaskboard))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0]; // Type
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType(controllerMethodReturnType);
    }

    [Fact]
    public async Task UpdateTaskboard_Returns_NoContent()
    {
        // Arrange
        var dto = AutoFaker.Generate<TaskboardUpdateCommand>();
        var id = Guid.NewGuid();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.UpdateTaskboard(id, dto, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteTaskboard_Returns_NoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.DeleteTaskboard(id, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AssignTaskboard_Returns_NoContent()
    {
        // Arrange
        var dto = AutoFaker.Generate<TaskboardAddUserCommand>();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.AssignTaskboard(dto, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UnassignTaskboard_Returns_NoContent()
    {
        // Arrange
        var dto = AutoFaker.Generate<TaskboardRemoveUserCommand>();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.UnassignTaskboard(dto, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
