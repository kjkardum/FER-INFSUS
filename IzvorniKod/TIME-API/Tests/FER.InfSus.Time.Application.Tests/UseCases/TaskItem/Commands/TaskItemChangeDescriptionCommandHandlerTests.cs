using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeDescription;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Commands;

public class TaskItemChangeDescriptionCommandHandlerTests
{
    private readonly TaskItemChangeDescriptionCommandHandler _taskItemChangeDescriptionCommandHandler;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserRepository _userRepository;

    public TaskItemChangeDescriptionCommandHandlerTests()
    {
        _taskItemRepository = Substitute.For<ITaskItemRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _taskItemChangeDescriptionCommandHandler = new TaskItemChangeDescriptionCommandHandler(
            _taskItemRepository,
            _userRepository);
    }

    [Fact]
    public async Task Handle_Valid_Request_Changes_Description_By_NormalUser_TaskboardMember()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeDescriptionCommand>();
        request.NewDescription = "New Description";
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
                            new() { UserId = request.RequestorId }
                        })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        await _taskItemChangeDescriptionCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).Update(taskItem, cancellationToken);
        await _taskItemRepository.Received(1).AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_Valid_Request_Changes_Description_By_AdminUser_NotTaskboardMember()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeDescriptionCommand>();
        request.NewDescription = "New Description";
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
                        new List<UserTaskboardAssociation>())
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        await _taskItemChangeDescriptionCommandHandler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).Update(taskItem, cancellationToken);
        await _taskItemRepository.Received(1).AddHistoryLog(Arg.Any<TaskItemHistoryLog>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_TaskItem_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeDescriptionCommand>();

        _taskItemRepository.GetById(request.Id, cancellationToken).Returns((Domain.Entities.TaskItem?)null);

        // Act
        var act = async () => await _taskItemChangeDescriptionCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }

    [Fact]
    public async Task Handle_TaskItem_Different_Tenant_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemChangeDescriptionCommand>();
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
        var act = async () => await _taskItemChangeDescriptionCommandHandler.Handle(request, cancellationToken);

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
        var request = AutoFaker.Generate<TaskItemChangeDescriptionCommand>();
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
        var act = async () => await _taskItemChangeDescriptionCommandHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }
}
