using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Rename;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Commands;

public class TaskItemRenameCommandHandlerTests
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly TaskItemRenameCommandHandler _taskItemRenameCommandHandler;

    public TaskItemRenameCommandHandlerTests()
    {
        _taskItemRepository = Substitute.For<ITaskItemRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _taskItemRenameCommandHandler = new TaskItemRenameCommandHandler(
            _taskItemRepository,
            _userRepository);
    }

    [Fact]
    public async Task Handle_Valid_Request_Renames_TaskItem()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemRenameCommand>();
        request.NewName = "New Name";
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.Name, "Old Name")
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
        await _taskItemRenameCommandHandler.Handle(request, cancellationToken);

        // Assert
        taskItem.Id.Should().Be(request.Id);
        taskItem.Name.Should().Be("New Name");
        await _userRepository.Received().GetByUserId(request.RequestorId);
        await _taskItemRepository.Received().GetById(request.Id, cancellationToken);
        await _taskItemRepository.Received().Update(taskItem, cancellationToken);
    }

    [Fact]
    public async Task Handle_Valid_Request_SameName_Doesnt_Rename_TaskItem()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemRenameCommand>();
        request.NewName = "Old Name";
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.Name, "Old Name")
            .RuleFor(
                t => t.Taskboard,
                new Faker<Domain.Entities.Taskboard>()
                    .RuleFor(t => t.TenantId, requestor.TenantId)
                    .RuleFor(
                        t => t.TaskboardUsers,
                        new List<UserTaskboardAssociation> { new() { UserId = request.RequestorId } })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        await _taskItemRenameCommandHandler.Handle(request, cancellationToken);

        // Assert
        taskItem.Id.Should().Be(request.Id);
        taskItem.Name.Should().Be("Old Name");
        await _userRepository.Received().GetByUserId(request.RequestorId);
        await _taskItemRepository.Received().GetById(request.Id, cancellationToken);
        await _taskItemRepository.DidNotReceive().Update(taskItem, cancellationToken);
    }

    [Fact]
    public async Task Handle_TaskItem_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemRenameCommand>();
        request.NewName = "New Name";
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).ReturnsNull();

        // Act
        var act = async () => await _taskItemRenameCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_Requestor_Not_Admin_And_Not_Taskboard_Member_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemRenameCommand>();
        request.NewName = "New Name";
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
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
                        f => new List<UserTaskboardAssociation>
                        {
                            new() { UserId = f.Random.Guid() }
                        })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var act = async () => await _taskItemRenameCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }

    [Fact]
    public async Task Handle_Different_Tenant_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemRenameCommand>();
        request.NewName = "New Name";
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, UserType.ADMIN)
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
        var act = async () => await _taskItemRenameCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
