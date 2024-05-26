using AutoBogus;
using AutoMapper;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByUserId;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.Taskboard.Queries;

public class TaskboardGetAllByUserIdQueryHandlerTests
{
    private readonly ITaskboardRepository _taskboardRepository;
    private readonly TaskboardGetAllByUserIdQueryHandler _taskboardGetAllByUserQueryHandler;

    public TaskboardGetAllByUserIdQueryHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new TaskboardProfile());
            cfg.AddProfile(new TaskItemProfile());
            cfg.AddProfile(new UserProfile());
        });
        var mapper = mapperConfiguration.CreateMapper();
        _taskboardRepository = Substitute.For<ITaskboardRepository>();
        _taskboardGetAllByUserQueryHandler = new TaskboardGetAllByUserIdQueryHandler(
            _taskboardRepository,
            mapper);
    }

    [Fact]
    public async Task Handle_Valid_Request_Returns_TaskboardSimpleDto_Collection()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskboardGetAllByUserIdQuery>();
        var taskboards = new Faker<Domain.Entities.Taskboard>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .Generate(10);

        _taskboardRepository.GetBoardsByUserId(request.RequestorId).Returns(taskboards);

        // Act
        var result = await _taskboardGetAllByUserQueryHandler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<TaskboardSimpleDto>>();
        result.Should().HaveCount(10);
        result.Select(t => t.Id).Should().BeEquivalentTo(taskboards.Select(t => t.Id));
    }
}
