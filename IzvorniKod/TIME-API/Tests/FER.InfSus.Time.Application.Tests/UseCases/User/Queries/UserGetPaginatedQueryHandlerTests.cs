using ApiExceptions.Exceptions;
using AutoBogus;
using AutoMapper;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Response;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Application.UseCases.User.Queries.GetPaginated;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.User.Queries;

public class UserGetPaginatedQueryHandlerTests
{
    private readonly UserGetPaginatedQueryHandler _userGetPaginatedQueryHandler;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserGetPaginatedQueryHandlerTests()
    {
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
        _mapper = mapperConfiguration.CreateMapper();
        _userRepository = Substitute.For<IUserRepository>();
        _userGetPaginatedQueryHandler = new UserGetPaginatedQueryHandler(_userRepository, _mapper);
    }

    [Fact]
    public async Task Handle_Valid_Request_Returns_PaginatedResponse()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserGetPaginatedQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.ADMIN)
            .RuleFor(u => u.TenantId, f => f.Random.Guid())
            .Generate();
        var users = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .Generate(10); // Generate 10 users
        var mappedUsers = users.ConvertAll(_mapper.Map<UserDto>);

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.GetPaginated(
                requestor.TenantId,
                request.Page,
                request.PageSize,
                request.OrderBy,
                request.FilterBy)
            .Returns(users);
        _userRepository.CountUsers().Returns(100); // Assume there are 100 total users

        // Act
        var result = await _userGetPaginatedQueryHandler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PaginatedResponse<UserDto>>();
        result.PageNumber.Should().Be(request.Page);
        result.PageSize.Should().Be(request.PageSize);
        result.TotalRecords.Should().Be(100);
        result.Data.Should().BeEquivalentTo(mappedUsers);
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1)
            .GetPaginated(requestor.TenantId, request.Page, request.PageSize, request.OrderBy, request.FilterBy);
        await _userRepository.Received(1).CountUsers();
    }

    [Fact]
    public async Task Handle_Requestor_Is_Not_Admin_Throws_ForbiddenAccessException()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserGetPaginatedQuery>();
        var requestor = new Faker<Domain.Entities.User>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.UserType, _ => UserType.USER)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

        // Act
        var act = async () => await _userGetPaginatedQueryHandler.Handle(request, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ForbiddenAccessException>();
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.DidNotReceive()
            .GetPaginated(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
        await _userRepository.DidNotReceive().CountUsers();
    }
}
