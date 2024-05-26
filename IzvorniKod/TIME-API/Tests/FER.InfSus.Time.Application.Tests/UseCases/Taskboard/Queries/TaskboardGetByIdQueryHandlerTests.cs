using ApiExceptions.Exceptions;
using AutoBogus;
using AutoMapper;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetById;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.Taskboard.Queries;

public class TaskboardGetByIdQueryHandlerTests
{
    private readonly ITaskboardRepository _taskboardRepository;
    private readonly IUserRepository _userRepository;
    private readonly TaskboardGetByIdQueryHandler _taskboardGetByIdQueryHandler;

    public TaskboardGetByIdQueryHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new TaskboardProfile());
            cfg.AddProfile(new TaskItemProfile());
            cfg.AddProfile(new UserProfile());
        });
        var mapper = mapperConfiguration.CreateMapper();
        _taskboardRepository = Substitute.For<ITaskboardRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _taskboardGetByIdQueryHandler = new TaskboardGetByIdQueryHandler(_taskboardRepository, _userRepository, mapper);
    }

    [Fact]
    public async Task Handle_ReturnsTaskboardDetailedDto_When_RequestorIsAdminAndNotTaskboardUser()
    {
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardGetByIdQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, _ => request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                f => new List<Domain.Entities.UserTaskboardAssociation> { new() {UserId = f.Random.Guid()} })
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.Id).Returns(taskboard);

        var result = await _taskboardGetByIdQueryHandler.Handle(request, cancellationToken);

        result.Should().NotBeNull();
        result.Should().BeOfType<TaskboardDetailedDto>();
        result.Id.Should().Be(taskboard.Id);
        result.Name.Should().Be(taskboard.Name);
        result.Description.Should().Be(taskboard.Description);
    }

    [Fact]
    public async Task Handle_ReturnsTaskboardDetailedDto_When_RequestorIsNotAdminAndIsTaskboardUser()
    {
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardGetByIdQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, _ => request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                _ => new List<Domain.Entities.UserTaskboardAssociation> { new() {UserId = requestor.Id} })
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.Id).Returns(taskboard);

        var result = await _taskboardGetByIdQueryHandler.Handle(request, cancellationToken);

        result.Should().NotBeNull();
        result.Should().BeOfType<TaskboardDetailedDto>();
        result.Id.Should().Be(taskboard.Id);
        result.Name.Should().Be(taskboard.Name);
        result.Description.Should().Be(taskboard.Description);
    }

    [Fact]
    public async Task Handle_ThrowsEntityNotFoundException_When_TaskboardIsNull()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardGetByIdQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, _ => request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.Id).Returns((Domain.Entities.Taskboard?)null);

        // Act
        var act = async () => await _taskboardGetByIdQueryHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.Id);
    }

    [Fact]
    public async Task Handle_ThrowsForbiddenAccessException_When_TenantIdsAreDifferent()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardGetByIdQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, _ => request.RequestorId)
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
        var act = async () => await _taskboardGetByIdQueryHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.Id);
    }

    [Fact]
    public async Task Handle_ThrowsForbiddenAccessException_When_RequestorIsNotAdminAndNotTaskboardUser()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardGetByIdQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, _ => request.RequestorId)
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboard = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, request.Id)
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .RuleFor(
                t => t.TaskboardUsers,
                f => new List<Domain.Entities.UserTaskboardAssociation> { new() { UserId = f.Random.Guid() } })
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetBoardById(request.Id).Returns(taskboard);

        // Act
        var act = async () => await _taskboardGetByIdQueryHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).GetBoardById(request.Id);
    }
}
