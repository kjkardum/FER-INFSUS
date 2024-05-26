using ApiExceptions.Exceptions;
using AutoMapper;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Create;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using NSubstitute;
using Xunit;
using FluentAssertions;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Commands;

public class TaskItemCreateCommandHandlerTests
{
    private readonly TaskItemCreateCommandHandler _handler;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITaskboardRepository _taskboardRepository;
    private readonly IUserRepository _userRepository;

    public TaskItemCreateCommandHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new TaskboardProfile());
            cfg.AddProfile(new TaskItemProfile());
            cfg.AddProfile(new UserProfile());
        });
        var mapper = mapperConfiguration.CreateMapper();
        _taskItemRepository = Substitute.For<ITaskItemRepository>();
        _taskboardRepository = Substitute.For<ITaskboardRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new TaskItemCreateCommandHandler(
            _taskItemRepository,
            _taskboardRepository,
            _userRepository,
            mapper);
    }

    [Fact]
    public async Task Handle_Valid_Request_Creates_TaskItem()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemCreateCommand>();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.TaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                new List<UserTaskboardAssociation> { new() { UserId = request.RequestorId } })
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.TaskboardId).Returns(taskboard);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).Create(Arg.Any<Domain.Entities.TaskItem>(), cancellationToken);
        result.Should().NotBeNull();
        result.Should().BeOfType<TaskItemSimpleDto>();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        result.State.Should().Be(TaskItemState.Novo);
    }

    [Fact]
    public async Task Handle_Taskboard_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemCreateCommand>();

        _taskboardRepository.GetBoardById(request.TaskboardId).Returns((Domain.Entities.Taskboard?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _taskboardRepository.Received(1).GetBoardById(request.TaskboardId);
    }

    [Fact]
    public async Task Handle_Different_Tenant_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemCreateCommand>();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.TaskboardId)
            .RuleFor(t => t.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.TaskboardId).Returns(taskboard);

        // Act
        Func<Task> act = async () => await _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.TaskboardId);
    }

    [Fact]
    public async Task Handle_Requestor_Not_Admin_And_Not_Taskboard_Member_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemCreateCommand>();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.TaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(t => t.TaskboardUsers, f => new List<UserTaskboardAssociation> { new() { UserId = f.Random.Guid() } })
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.TaskboardId).Returns(taskboard);

        // Act
        Func<Task> act = async () => await _handler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.TaskboardId);
    }

    [Fact]
    public async Task Handle_Valid_Admin_Creates_TaskItem()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemCreateCommand>();

        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.TaskboardId)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.TaskboardId).Returns(taskboard);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).Create(Arg.Any<Domain.Entities.TaskItem>(), cancellationToken);
        result.Should().NotBeNull();
        result.Should().BeOfType<TaskItemSimpleDto>();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        result.State.Should().Be(TaskItemState.Novo);
    }
}
