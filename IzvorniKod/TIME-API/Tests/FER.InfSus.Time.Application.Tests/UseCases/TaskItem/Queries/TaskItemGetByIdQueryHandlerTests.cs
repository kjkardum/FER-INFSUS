using ApiExceptions.Exceptions;
using AutoBogus;
using AutoMapper;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetById;
using FER.InfSus.Time.Domain;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Queries;

public class TaskItemGetByIdQueryHandlerTests
{
    private readonly TaskItemGetByIdQueryHandler _taskItemGetByIdQueryHandler;
    private readonly IUserRepository _userRepository;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IMapper _mapper;

    public TaskItemGetByIdQueryHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TaskboardProfile>();
            cfg.AddProfile<TaskItemProfile>();
            cfg.AddProfile<UserProfile>();
        });
        _mapper = mapperConfiguration.CreateMapper();
        _userRepository = Substitute.For<IUserRepository>();
        _taskItemRepository = Substitute.For<ITaskItemRepository>();
        _taskItemGetByIdQueryHandler = new TaskItemGetByIdQueryHandler(
            _userRepository,
            _taskItemRepository,
            _mapper);
    }

    [Fact]
    public async Task Handle_Valid_Request_Not_Admin_And_Taskboard_Member_Returns_TaskItemDetailedDto()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemGetByIdQuery>();
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
                        new List<UserTaskboardAssociation> { new() { UserId = request.RequestorId } })
                    .Generate())
            .Generate();
        var mappedTaskItem = _mapper.Map<TaskItemDetailedDto>(taskItem);

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var result = await _taskItemGetByIdQueryHandler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedTaskItem);
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }

    [Fact]
    public async Task Handle_Valid_Request_Admin_And_Not_Taskboard_Member_Returns_TaskItemDetailedDto()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemGetByIdQuery>();
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
                        f => new List<UserTaskboardAssociation> { new() { UserId = f.Random.Guid() } })
                    .Generate())
            .Generate();
        var mappedTaskItem = _mapper.Map<TaskItemDetailedDto>(taskItem);

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var result = await _taskItemGetByIdQueryHandler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(mappedTaskItem);
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }

    [Fact]
    public async Task Handle_TaskItem_Not_Found_Throws_EntityNotFoundException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemGetByIdQuery>();

        _taskItemRepository.GetById(request.Id, cancellationToken).Returns((Domain.Entities.TaskItem?)null);

        // Act
        var act = async () => await _taskItemGetByIdQueryHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }

    [Fact]
    public async Task Handle_TaskItem_Different_Tenant_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemGetByIdQuery>();
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
                    .RuleFor(
                        t => t.TaskboardUsers,
                        new List<UserTaskboardAssociation> { new() { UserId = request.RequestorId } })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var act = async () => await _taskItemGetByIdQueryHandler.Handle(request, cancellationToken);

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
        var request = AutoFaker.Generate<TaskItemGetByIdQuery>();
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
                        f => new List<UserTaskboardAssociation> { new() { UserId = f.Random.Guid() } })
                    .Generate())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskItemRepository.GetById(request.Id, cancellationToken).Returns(taskItem);

        // Act
        var act = async () => await _taskItemGetByIdQueryHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskItemRepository.Received(1).GetById(request.Id, cancellationToken);
    }
}
