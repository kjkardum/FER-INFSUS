using ApiExceptions.Exceptions;
using AutoBogus;
using AutoMapper;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Create;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.Taskboard.Commands;

public class TaskboardCreateCommandHandlerTests
{
    private readonly TaskboardCreateCommandHandler _taskboardCreateCommandHandler;
    private readonly ITaskboardRepository _taskboardRepository;
    private readonly IUserRepository _userRepository;

    public TaskboardCreateCommandHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<TaskboardProfile>());
        var mapper = mapperConfiguration.CreateMapper();
        _taskboardRepository = Substitute.For<ITaskboardRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _taskboardCreateCommandHandler = new TaskboardCreateCommandHandler(
            _taskboardRepository,
            _userRepository,
            mapper);
    }

    [Fact]
    public async Task Handle_Valid_Request_Returns_TaskboardSimpleDto()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardCreateCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

        // Act
        var result = await _taskboardCreateCommandHandler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.Received(1).CreateBoard(Arg.Is<Domain.Entities.Taskboard>(t =>
            t.Name == request.Name &&
                t.Description == request.Description &&
                t.TenantId == requestor.TenantId));
    }

    [Fact]
    public async Task Handle_Requestor_Is_Not_Admin_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardCreateCommand>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

        // Act
        var act = async () => await _taskboardCreateCommandHandler.Handle(request, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(act);
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _taskboardRepository.DidNotReceive().CreateBoard(Arg.Any<Domain.Entities.Taskboard>());
    }
}
