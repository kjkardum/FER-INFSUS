using ApiExceptions.Exceptions;
using AutoBogus;
using AutoMapper;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByTenant;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.Taskboard.Queries;

public class TaskboardGetAllByTenantQueryHandlerTests
{
    private readonly ITaskboardRepository _taskboardRepository;
    private readonly IUserRepository _userRepository;
    private readonly TaskboardGetAllByTenantQueryHandler _taskboardGetAllByTenantQueryHandler;

    public TaskboardGetAllByTenantQueryHandlerTests()
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
        _taskboardGetAllByTenantQueryHandler = new TaskboardGetAllByTenantQueryHandler(
            _taskboardRepository,
            _userRepository,
            mapper);
    }

    [Fact]
    public async Task Handle_Valid_Request_Returns_TaskboardSimpleDto_Collection()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardGetAllByTenantQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var taskboards = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.TenantId, requestor.TenantId)
            .Generate(10);

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _taskboardRepository.GetTenantBoards(requestor.TenantId).Returns(taskboards);

        // Act
        var result = await _taskboardGetAllByTenantQueryHandler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<TaskboardSimpleDto>>();
        result.Count.Should().Be(10);
        result.Select(t => t.Id).Should().BeEquivalentTo(taskboards.Select(t => t.Id));
    }

    [Fact]
    public async Task Handle_RequestorIsNotAdmin_ThrowsForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardGetAllByTenantQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

        // Act
        var action = new Func<Task>(() => _taskboardGetAllByTenantQueryHandler.Handle(request, cancellationToken));

        // Assert
        await action.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
