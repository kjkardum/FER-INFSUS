using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Update;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.Taskboard.Commands;

public class TaskboardUpdateCommandHandlerTests
{
    private readonly ITaskboardRepository _taskboardRepository;
    private readonly IUserRepository _userRepository;
    private readonly TaskboardUpdateCommandHandler _taskboardUpdateCommandHandler;

    public TaskboardUpdateCommandHandlerTests()
    {
        _taskboardRepository = Substitute.For<ITaskboardRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _taskboardUpdateCommandHandler = new TaskboardUpdateCommandHandler(
            _taskboardRepository,
            _userRepository);
    }

    [Fact]
    public async Task Handle_Valid_Request_Succeeds()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardUpdateCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.Id).Returns(taskboard);

        // Act
        await _taskboardUpdateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.Id);
        await _taskboardRepository.Received(1).UpdateBoard(taskboard);
    }

    [Fact]
    public async Task Handle_RequestorIsNotAdmin_ThrowsForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardUpdateCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

        // Act
        var act = async () => await _taskboardUpdateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.DidNotReceive().GetBoardById(Arg.Any<Guid>());
        await _taskboardRepository.DidNotReceive().UpdateBoard(Arg.Any<Domain.Entities.Taskboard>());
    }

    [Fact]
    public async Task Handle_TaskboardNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardUpdateCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.Id).ReturnsNull();

        // Act
        var act = async () => await _taskboardUpdateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.Id);
        await _taskboardRepository.DidNotReceive().UpdateBoard(Arg.Any<Domain.Entities.Taskboard>());
    }

    [Fact]
    public async Task Handle_RequestorDoesNotHavePermission_ThrowsForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardUpdateCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.Id).Returns(taskboard);

        // Act
        var act = async () => await _taskboardUpdateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.Id);
        await _taskboardRepository.DidNotReceive().UpdateBoard(Arg.Any<Domain.Entities.Taskboard>());
    }
}
