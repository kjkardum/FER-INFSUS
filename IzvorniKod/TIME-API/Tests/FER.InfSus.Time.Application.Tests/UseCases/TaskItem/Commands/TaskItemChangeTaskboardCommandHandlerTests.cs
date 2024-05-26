using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeTaskboard;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using NSubstitute;
using Xunit;
using FluentAssertions;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Commands;

public class TaskItemChangeTaskboardCommandHandlerTests
{
    private readonly TaskItemChangeTaskboardCommandHandler _handler;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITaskboardRepository _taskboardRepository;
    private readonly IUserRepository _userRepository;

    public TaskItemChangeTaskboardCommandHandlerTests()
    {
        _taskItemRepository = Substitute.For<ITaskItemRepository>();
        _taskboardRepository = Substitute.For<ITaskboardRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new TaskItemChangeTaskboardCommandHandler(
            _taskItemRepository,
            _taskboardRepository,
            _userRepository);
    }

    [Fact]
    public async Task Handle_Valid_Request_Changes_Taskboard()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();
        request.NewTaskboardId = Guid.NewGuid();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var oldTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                new List<UserTaskboardAssociation> { new() { UserId = request.RequestorId } })
            .Generate();

        var newTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.NewTaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                new List<UserTaskboardAssociation> { new() { UserId = request.RequestorId } })
            .Generate();

        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TaskboardId, oldTaskboard.Id)
            .RuleFor(t => t.Taskboard, oldTaskboard)
            .RuleFor(t => t.AssignedUserId, (Guid?)null)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);
        _taskboardRepository.GetBoardById(request.NewTaskboardId).Returns(newTaskboard);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).Update(taskItem, cancellationToken);
        await _taskItemRepository.Received(1).AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
        taskItem.TaskboardId.Should().Be(request.NewTaskboardId);
    }

    [Fact]
    public async Task Handle_Valid_Admin_Request_Changes_Taskboard()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();
        request.NewTaskboardId = Guid.NewGuid();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var oldTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                new List<UserTaskboardAssociation>())
            .Generate();

        var newTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.NewTaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                new List<UserTaskboardAssociation>())
            .Generate();

        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TaskboardId, oldTaskboard.Id)
            .RuleFor(t => t.Taskboard, oldTaskboard)
            .RuleFor(t => t.AssignedUserId, (Guid?)null)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);
        _taskboardRepository.GetBoardById(request.NewTaskboardId).Returns(newTaskboard);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).Update(taskItem, cancellationToken);
        await _taskItemRepository.Received(1).AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
        taskItem.TaskboardId.Should().Be(request.NewTaskboardId);
    }

    [Fact]
    public async Task Handle_TaskItem_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();

        _taskItemRepository.GetById(request.Id, cancellationToken).Returns((Domain.Entities.TaskItem?)null);

        // Act
        var act = async () => await _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }

    [Fact]
    public async Task Handle_NewTaskboard_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();

        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .Generate();

        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);
        _taskboardRepository.GetBoardById(request.NewTaskboardId).Returns((Domain.Entities.Taskboard?)null);

        // Act
        var act = async () => await _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _taskboardRepository.Received(1).GetBoardById(request.NewTaskboardId);
    }

    [Fact]
    public async Task Handle_Taskboards_From_Different_Tenants_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(
                t => t.Taskboard,
                new Faker<Domain.Entities.Taskboard>()
                    .RuleFor(t => t.TenantId, f => f.Random.Guid())
                    .Generate())
            .Generate();

        var newTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.NewTaskboardId)
            .RuleFor(t => t.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);
        _taskboardRepository.GetBoardById(request.NewTaskboardId).Returns(newTaskboard);

        // Act
        var act = async () => await _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
        await _taskboardRepository.Received(1).GetBoardById(request.NewTaskboardId);
    }

    [Fact]
    public async Task Handle_Requestor_Not_Admin_And_Not_Taskboard_Member_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var oldTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                f => new List<UserTaskboardAssociation> { new() { UserId = f.Random.Guid() } })
            .Generate();

        var newTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.NewTaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                f => new List<UserTaskboardAssociation> { new() { UserId = f.Random.Guid() } })
            .Generate();

        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TaskboardId, oldTaskboard.Id)
            .RuleFor(t => t.Taskboard, oldTaskboard)
            .RuleFor(t => t.AssignedUserId, (Guid?)null)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);
        _taskboardRepository.GetBoardById(request.NewTaskboardId).Returns(newTaskboard);

        // Act
        var act = async () => await _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
        await _taskboardRepository.Received(1).GetBoardById(request.NewTaskboardId);
    }

    [Fact]
    public async Task Handle_TaskItem_AssignedUser_Not_Member_Of_NewTaskboard_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var oldTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                new List<UserTaskboardAssociation> { new() { UserId = request.RequestorId } })
            .Generate();

        var newTaskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.NewTaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(t => t.TaskboardUsers, new List<UserTaskboardAssociation>())
            .Generate();

        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TaskboardId, oldTaskboard.Id)
            .RuleFor(t => t.Taskboard, oldTaskboard)
            .RuleFor(t => t.AssignedUserId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);
        _taskboardRepository.GetBoardById(request.NewTaskboardId).Returns(newTaskboard);

        // Act
        var act = async () => await _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
        await _taskboardRepository.Received(1).GetBoardById(request.NewTaskboardId);
    }

    [Fact]
    public async Task Handle_TaskboardId_Not_Changed_No_Update_Called()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeTaskboardCommand>();
        request.NewTaskboardId = Guid.NewGuid();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.NewTaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .Generate();

        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TaskboardId, taskboard.Id)
            .RuleFor(t => t.Taskboard, taskboard)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.DidNotReceive().Update(taskItem, cancellationToken);
        await _taskItemRepository.DidNotReceive().AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
    }
}
