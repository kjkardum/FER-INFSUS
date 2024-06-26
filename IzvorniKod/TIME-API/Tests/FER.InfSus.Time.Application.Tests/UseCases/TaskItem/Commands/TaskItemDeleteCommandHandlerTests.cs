using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Delete;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Commands;

public class TaskItemDeleteCommandHandlerTests
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly TaskItemDeleteCommandHandler _taskItemDeleteCommandHandler;

    public TaskItemDeleteCommandHandlerTests()
    {
        _taskItemRepository = Substitute.For<ITaskItemRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _taskItemDeleteCommandHandler = new TaskItemDeleteCommandHandler(
            _taskItemRepository,
            _userRepository);
    }

    [Fact]
    public async Task Handle_Valid_Request_Deletes_TaskItem()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemDeleteCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(
                t => t.Taskboard,
                new Faker<Domain.Entities.Taskboard>()
                    .RuleFor(t => t.TenantId, requestor.TenantId)
                    .RuleFor(
                        t => t.TaskboardUsers,
                        f =>new List<UserTaskboardAssociation>
                        {
                            new() { UserId = f.Random.Guid() }
                        })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        await _taskItemDeleteCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
        await _taskItemRepository.Received(1).Delete(taskItem, cancellationToken);
    }

    [Fact]
    public async Task Handle_TaskItem_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemDeleteCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).ReturnsNull();

        // Act
        var act = async () => await _taskItemDeleteCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
        await _taskItemRepository.DidNotReceive().Delete(Arg.Any<Domain.Entities.TaskItem>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_Requestor_Not_Admin_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemDeleteCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(
                t => t.Taskboard,
                new Faker<Domain.Entities.Taskboard>()
                    .RuleFor(t => t.TenantId, requestor.TenantId)
                    .RuleFor(
                        t => t.TaskboardUsers,
                        new List<UserTaskboardAssociation>
                        {
                            new() { UserId = request.RequestorId }
                        })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var act = async () => await _taskItemDeleteCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
        await _taskItemRepository.DidNotReceive().Delete(Arg.Any<Domain.Entities.TaskItem>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_Different_Tenant_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemDeleteCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
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

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var act = async () => await _taskItemDeleteCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
        await _taskItemRepository.DidNotReceive().Delete(Arg.Any<Domain.Entities.TaskItem>(), cancellationToken);
    }
}
