using AutoBogus;
using AutoMapper;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetAllByAssignedUser;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Queries;

public class TaskItemGetAllByAssignedUserQueryHandlerTests
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly TaskItemGetAllByAssignedUserQueryHandler _taskItemGetAllByAssignedUserQueryHandler;

    public TaskItemGetAllByAssignedUserQueryHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new TaskboardProfile());
            cfg.AddProfile(new TaskItemProfile());
            cfg.AddProfile(new UserProfile());
        });
        var mapper = mapperConfiguration.CreateMapper();
        _taskItemRepository = Substitute.For<ITaskItemRepository>();
        _taskItemGetAllByAssignedUserQueryHandler = new TaskItemGetAllByAssignedUserQueryHandler(
            _taskItemRepository,
            mapper);
    }

    [Fact]
    public async Task Handle_Valid_Request_Returns_Collection_Of_TaskItemForTasklistDto()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<TaskItemGetAllByAssignedUserQuery>();
        var taskItems = new Faker<Domain.Entities.TaskItem>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .Generate(5);

        _taskItemRepository.GetByAssignedUserId(request.RequestorId).Returns(taskItems);

        // Act
        var result = await _taskItemGetAllByAssignedUserQueryHandler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<TaskItemForTasklistDto>>();
        result.Should().HaveCount(5);
        result.Select(t => t.Id).Should().BeEquivalentTo(taskItems.Select(t => t.Id));
    }
}
