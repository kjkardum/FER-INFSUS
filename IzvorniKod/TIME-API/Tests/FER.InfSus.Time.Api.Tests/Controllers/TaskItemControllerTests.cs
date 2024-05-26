using AutoBogus;
using FER.InfSus.Time.Api.Controllers;
using FER.InfSus.Time.Api.Services;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Assign;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeDescription;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeState;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeTaskboard;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Create;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Delete;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Rename;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetAllByAssignedUser;
using FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections;
using Xunit;

namespace FER.InfSus.Time.Api.Tests.Controllers;

public class TaskItemControllerTests
{
    private readonly TaskItemController _controller;
    private readonly IMediator _mediator;
    private readonly IAuthenticationService _authenticationService;

    public TaskItemControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _authenticationService = Substitute.For<IAuthenticationService>();
        _controller = new TaskItemController(_mediator, _authenticationService);
    }

    [Fact]
    public async Task GetTaskItem_Returns_TaskItemDetailedDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<TaskItemDetailedDto>();

        _authenticationService.GetUserId().Returns(Guid.NewGuid());
        _mediator.Send(Arg.Any<TaskItemGetByIdQuery>(), cancellationToken).Returns(response);

        // Act
        var result = await _controller.GetTaskItem(id, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        var controllerMethodReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.GetTaskItem))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0]; // Type
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType(controllerMethodReturnType);
    }

    [Fact]
    public async Task GetAssignedTaskItems_Returns_TaskItemSimpleDtoCollection()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<TaskItemForTasklistDto>(5);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());
        _mediator.Send(Arg.Any<TaskItemGetAllByAssignedUserQuery>(), cancellationToken).Returns(response);

        // Act
        var result = await _controller.GetAssignedTaskItems(cancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var responseResult = result.Result as OkObjectResult;
        var controllerMethodSingleItemReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.GetAssignedTaskItems))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0] // Type (ICollection<TaskItemForTasklistDto>)
            .GetGenericArguments()[0]; // Type (TaskItemForTasklistDto)
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType<List<TaskItemForTasklistDto>>();
        (responseResult!.Value as IList)![0].Should().BeOfType(controllerMethodSingleItemReturnType);
    }

    [Fact]
    public async Task CreateTaskItem_Returns_TaskItemDetailedDto()
    {
        // Arrange
        var command = AutoFaker.Generate<TaskItemCreateCommand>();
        var cancellationToken = default(CancellationToken);
        var response = AutoFaker.Generate<TaskItemSimpleDto>();

        _authenticationService.GetUserId().Returns(Guid.NewGuid());
        _mediator.Send(Arg.Any<TaskItemCreateCommand>(), cancellationToken).Returns(response);

        // Act
        var result = await _controller.CreateTaskItem(command, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var responseResult = result.Result as CreatedAtActionResult;
        var controllerMethodReturnType = _controller
            .GetType()
            .GetMethod(nameof(_controller.CreateTaskItem))!
            .ReturnType // Task<ActionResult<Type>>
            .GetGenericArguments()[0] // ActionResult<Type>
            .GetGenericArguments()[0]; // Type
        responseResult!.Value.Should().BeEquivalentTo(response);
        responseResult!.Value.Should().BeOfType(controllerMethodReturnType);
    }

    [Fact]
    public async Task AssignTaskItem_Returns_NoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = AutoFaker.Generate<TaskItemAssignCommand>();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.AssignTaskItem(id, command, cancellationToken);

        // Assert
        await _mediator.Received(1).Send(Arg.Any<TaskItemAssignCommand>(), cancellationToken);
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task RenameTaskItem_Returns_NoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = AutoFaker.Generate<TaskItemRenameCommand>();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.RenameTaskItem(id, command, cancellationToken);

        // Assert
        await _mediator.Received(1).Send(Arg.Any<TaskItemRenameCommand>(), cancellationToken);
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ChangeTaskItemDescription_Returns_NoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = AutoFaker.Generate<TaskItemChangeDescriptionCommand>();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.ChangeTaskItemDescription(id, command, cancellationToken);

        // Assert
        await _mediator.Received(1).Send(Arg.Any<TaskItemChangeDescriptionCommand>(), cancellationToken);
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task SetTaskItemState_Returns_NoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = AutoFaker.Generate<TaskItemChangeStateCommand>();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.SetTaskItemState(id, command, cancellationToken);

        // Assert
        await _mediator.Received(1).Send(Arg.Any<TaskItemChangeStateCommand>(), cancellationToken);
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task MoveTaskItem_Returns_NoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.MoveTaskItem(id, command, cancellationToken);

        // Assert
        await _mediator.Received(1).Send(Arg.Any<TaskItemChangeTaskboardCommand>(), cancellationToken);
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteTaskItem_Returns_NoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = default(CancellationToken);

        _authenticationService.GetUserId().Returns(Guid.NewGuid());

        // Act
        var result = await _controller.DeleteTaskItem(id, cancellationToken);

        // Assert
        await _mediator.Received(1).Send(Arg.Any<TaskItemDeleteCommand>(), cancellationToken);
        result.Should().BeOfType<NoContentResult>();
    }
}
