using AutoBogus;
using AutoMapper;
using Bogus;
using FER.InfSus.Time.Application.Mappings;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Application.UseCases.User.Commands.Create;
using FER.InfSus.Time.Application.UseCases.User.Commands.Login;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using MediatR;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.User.Commands;

public class UserCreateCommandHandlerTests
{
    private readonly IRequestHandler<UserRegistrationCommand> _userRegistrationCommandHandler;
    private readonly IUserRepository _userRepository;
    private readonly ISignInService _signInService;

    public UserCreateCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _signInService = Substitute.For<ISignInService>();

        _userRegistrationCommandHandler = new UserRegistrationCommandHandler(_userRepository, _signInService);
    }

    [Fact]
    public async Task UserCreateCommandHandler_Creates_New_User()
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

        _userRepository
            .Create(Arg.Any<Domain.Entities.User>())
            .Returns(createdUser);

        // Act
        await _userRegistrationCommandHandler.Handle(request, cancellationToken);

        //Assert
        await _userRepository
            .Received(1)
            .Create(Arg.Any<Domain.Entities.User>());
    }
}
