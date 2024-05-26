using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Application.UseCases.User.Commands.Create;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.User.Commands;

public class UserRegistrationCommandHandlerTests
{
    private readonly UserRegistrationCommandHandler _userRegistrationCommandHandler;
    private readonly IUserRepository _userRepository;
    private readonly ISignInService _signInService;

    public UserRegistrationCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _signInService = Substitute.For<ISignInService>();

        _userRegistrationCommandHandler = new UserRegistrationCommandHandler(_userRepository, _signInService);
    }

    [Fact]
    public async Task UserCreateCommandHandler_Valid_Request_Creates_New_User()
    {
        //Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserRegistrationCommand>();
        var createdUser = new Faker<Domain.Entities.User>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.Email, _ => request.NormalizedEmail)
            .RuleFor(t => t.FirstName, f => f.Person.FirstName)
            .RuleFor(t => t.LastName, f => f.Person.LastName)
            .Generate();
        var requestor = new Faker<Domain.Entities.User>().RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.UserType, _ => Domain.Enums.UserType.ADMIN)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.DoesUserExist(request.NormalizedEmail).Returns(false);
        _signInService.HashPassword(request.Password).Returns("hashedPassword");
        _userRepository
            .Create(Arg.Any<Domain.Entities.User>())
            .Returns(createdUser);

        // Act
        await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).DoesUserExist(request.NormalizedEmail);
        _signInService.Received(1).HashPassword(request.Password);
        await _userRepository.Received(1).Create(Arg.Is<Domain.Entities.User>(user =>
            user.TenantId == requestor.TenantId &&
                user.Email == request.NormalizedEmail &&
                user.PasswordHash == "hashedPassword" &&
                user.FirstName == request.FirstName &&
                user.LastName == request.LastName &&
                user.UserType == request.UserType &&
                user.DateOfBirth == request.DateOfBirth
        ));
    }

    [Fact]
    public async Task UserCreateCommandHandler_Requestor_Is_Not_Admin_Throws_ForbiddenAccessException()
    {
        //Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserRegistrationCommand>();
        var requestor = new Faker<Domain.Entities.User>().RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.UserType, _ => Domain.Enums.UserType.USER)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

        // Act
        var act = async () => await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(act);
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.DidNotReceive().DoesUserExist(request.NormalizedEmail);
        _signInService.DidNotReceive().HashPassword(request.Password);
        await _userRepository.DidNotReceive().Create(Arg.Any<Domain.Entities.User>());
    }

    [Fact]
    public async Task UserCreateCommandHandler_User_With_Same_Email_Already_Exists_Throws_EntityAlreadyExistsException()
    {
        //Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserRegistrationCommand>();
        var requestor = new Faker<Domain.Entities.User>().RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.UserType, _ => Domain.Enums.UserType.ADMIN)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.DoesUserExist(request.NormalizedEmail).Returns(true);

        // Act
        var act = async () => await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await Assert.ThrowsAsync<EntityAlreadyExistsException>(act);
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).DoesUserExist(request.NormalizedEmail);
        _signInService.DidNotReceive().HashPassword(request.Password);
        await _userRepository.DidNotReceive().Create(Arg.Any<Domain.Entities.User>());
    }

    [Fact]
    public async Task UserCreateCommandHandler_HashPassword_Returns_Null_Throws_BadRequestException()
    {
        //Arrange
        var cancellationToken = default(CancellationToken);
        var request = AutoFaker.Generate<UserRegistrationCommand>();
        var requestor = new Faker<Domain.Entities.User>().RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.UserType, _ => Domain.Enums.UserType.ADMIN)
            .Generate();

        _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
        _userRepository.DoesUserExist(request.NormalizedEmail).Returns(false);
        _signInService.HashPassword(request.Password).Returns((string?)null);

        // Act
        var act = async () => await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await Assert.ThrowsAsync<BadRequestException>(act);
        await _userRepository.Received(1).GetByUserId(request.RequestorId);
        await _userRepository.Received(1).DoesUserExist(request.NormalizedEmail);
        _signInService.Received(1).HashPassword(request.Password);
        await _userRepository.DidNotReceive().Create(Arg.Any<Domain.Entities.User>());
    }
}
