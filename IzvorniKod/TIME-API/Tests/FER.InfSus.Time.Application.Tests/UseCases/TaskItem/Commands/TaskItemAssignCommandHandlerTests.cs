using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Assign;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Commands;

public class TaskItemAssignCommandHandlerTests
{
    private readonly TaskItemAssignCommandHandler _taskItemAssignCommandHandler;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserRepository _userRepository;

    public TaskItemAssignCommandHandlerTests()
    {
        _taskItemRepository = Substitute.For<ITaskItemRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _taskItemAssignCommandHandler = new TaskItemAssignCommandHandler(_taskItemRepository, _userRepository);
    }

    [Fact]
    public async Task Handle_Valid_Request_Assigns_WhenId_NotNull_By_NormalUser_TaskboardMember()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemAssignCommand>();
        request.AssignedUserId = Guid.NewGuid();
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
                        new List<UserTaskboardAssociation>
                        {
                            new() { UserId = request.RequestorId },
                            new() { UserId = (Guid)request.AssignedUserId! }
                        })
                    .Generate())
            .Generate();
        var assignedUser = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, (Guid)request.AssignedUserId!)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);
        _userRepository.GetByUserId((Guid)request.AssignedUserId!).Returns(assignedUser);

        // Act
        await _taskItemAssignCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(2).GetByUserId(Arg.Any<Guid>());
        await _taskItemRepository.Received(1).Update(taskItem, cancellationToken);
        await _taskItemRepository.Received(1).AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_Valid_Request_Unassigns_WhenId_Null_By_NormalUser_TaskboardMember()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemAssignCommand>();
        request.AssignedUserId = null;
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.AssignedUserId, f => f.Random.Guid())
            .RuleFor(
                t => t.Taskboard,
                new Faker<Domain.Entities.Taskboard>()
                    .RuleFor(t => t.TenantId, requestor.TenantId)
                    .RuleFor(
                        t => t.TaskboardUsers,
                        new List<UserTaskboardAssociation>
                        {
                            new() { UserId = request.RequestorId },
                        })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        await _taskItemAssignCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(Arg.Any<Guid>());
        await _taskItemRepository.Received(1).Update(taskItem, cancellationToken);
        await _taskItemRepository.Received(1).AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_Valid_Request_Unassigns_WhenId_Null_By_AdminUser_NotTaskboardMember()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemAssignCommand>();
        request.AssignedUserId = null;
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskItem = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.AssignedUserId, f => f.Random.Guid())
            .RuleFor(
                t => t.Taskboard,
                new Faker<Domain.Entities.Taskboard>()
                    .RuleFor(t => t.TenantId, requestor.TenantId)
                    .RuleFor(
                        t => t.TaskboardUsers,
                        new List<UserTaskboardAssociation>())
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        await _taskItemAssignCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(Arg.Any<Guid>());
        await _taskItemRepository.Received(1).Update(taskItem, cancellationToken);
        await _taskItemRepository.Received(1).AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_Valid_Request_Assigns_WhenId_NotNull_By_AdminUser_NotTaskboardMember()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemAssignCommand>();
        request.AssignedUserId = Guid.NewGuid();
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
                        new List<UserTaskboardAssociation>
                        {
                            new() { UserId = request.AssignedUserId!.Value },
                        })
                    .Generate())
            .Generate();
        var assignedUser = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, (Guid)request.AssignedUserId!)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);
        _userRepository.GetByUserId((Guid)request.AssignedUserId!).Returns(assignedUser);

        // Act
        await _taskItemAssignCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(2).GetByUserId(Arg.Any<Guid>());
        await _taskItemRepository.Received(1).Update(taskItem, cancellationToken);
        await _taskItemRepository.Received(1).AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_TaskItem_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemAssignCommand>();

        _taskItemRepository.GetById(request.Id, cancellationToken).Returns((Domain.Entities.TaskItem?)null);

        // Act
        var act = async () => await _taskItemAssignCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }

    [Fact]
    public async Task Handle_TaskItem_Different_Tenant_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemAssignCommand>();
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

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var act = async () => await _taskItemAssignCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }

    [Fact]
    public async Task Handle_Requestor_Not_Admin_And_Not_Taskboard_Member_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemAssignCommand>();
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
        var act = async () => await _taskItemAssignCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }

    [Fact]
    public async Task Handle_AssignedUser_Not_Member_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemAssignCommand>();
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
                        f => new List<UserTaskboardAssociation>
                        {
                            new() { UserId = request.RequestorId }
                        })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var act = async () => await _taskItemAssignCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }
}
