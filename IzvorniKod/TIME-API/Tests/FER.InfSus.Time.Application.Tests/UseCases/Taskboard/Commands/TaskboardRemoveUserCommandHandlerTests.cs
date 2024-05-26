using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.RemoveUser;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.Taskboard.Commands;

public class TaskboardRemoveUserCommandHandlerTests
{
    private readonly ITaskboardRepository _taskboardRepository;
    private readonly IUserRepository _userRepository;
    private readonly TaskboardRemoveUserCommandHandler _taskboardRemoveUserCommandHandler;

    public TaskboardRemoveUserCommandHandlerTests()
    {
        _taskboardRepository = Substitute.For<ITaskboardRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _taskboardRemoveUserCommandHandler = new TaskboardRemoveUserCommandHandler(
            _taskboardRepository,
            _userRepository);
    }

    [Fact]
    public async Task Handle_Valid_Request_Succeeds()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardRemoveUserCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.TaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                _ => new List<UserTaskboardAssociation> { new() { UserId = request.UserId } })
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.TaskboardId).Returns(taskboard);

        // Act
        await _taskboardRemoveUserCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.TaskboardId);
        await _taskboardRepository.Received(1).RemoveUserFromBoard(request.TaskboardId, request.UserId);
    }

    [Fact]
    public async Task Handle_RequestorIsNotAdmin_ThrowsForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardRemoveUserCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

        // Act
        var act = async () => await _taskboardRemoveUserCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.DidNotReceive().GetBoardById(Arg.Any<Guid>());
        await _taskboardRepository.DidNotReceive().RemoveUserFromBoard(Arg.Any<Guid>(), Arg.Any<Guid>());
    }

    [Fact]
    public async Task Handle_TaskboardNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardRemoveUserCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.TaskboardId).ReturnsNull();

        // Act
        var act = async () => await _taskboardRemoveUserCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.TaskboardId);
        await _taskboardRepository.DidNotReceive().RemoveUserFromBoard(Arg.Any<Guid>(), Arg.Any<Guid>());
    }

    [Fact]
    public async Task Handle_Taskboard_TenantId_DoesNotMatch_Requestor_TenantId_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardRemoveUserCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, f => request.TaskboardId)
            .RuleFor(t => t.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.TaskboardId).Returns(taskboard);

        // Act
        var act = async () => await _taskboardRemoveUserCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.TaskboardId);
        await _taskboardRepository.DidNotReceive().RemoveUserFromBoard(Arg.Any<Guid>(), Arg.Any<Guid>());
    }

    [Fact]
    public async Task Handle_User_Not_In_Taskboard_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardRemoveUserCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, _ => request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, _ => request.TaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                f => new List<UserTaskboardAssociation> { new() { UserId = f.Random.Guid() } })
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.TaskboardId).Returns(taskboard);

        // Act
        var act = async () => await _taskboardRemoveUserCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.TaskboardId);
        await _taskboardRepository.DidNotReceive().RemoveUserFromBoard(Arg.Any<Guid>(), Arg.Any<Guid>());
    }
}
