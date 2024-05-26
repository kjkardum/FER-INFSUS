using ApiExceptions.Exceptions;
using AutoBogus;
using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.User.Commands.Delete;
using FER.InfSus.Time.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.User.Commands
{
    public class UserDeleteCommandHandlerTests
    {
        private readonly UserDeleteCommandHandler _userDeleteCommandHandler;
        private readonly IUserRepository _userRepository;

        public UserDeleteCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            _userDeleteCommandHandler = new UserDeleteCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Handle_Valid_Request_Deletes_User()
        {
            // Arrange
            var cancellationToken = default(CancellationToken);
            var request = AutoFaker.Generate<UserDeleteCommand>();
            var requestor = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.UserType, _ => UserType.ADMIN)
                .RuleFor(u => u.TenantId, f => f.Random.Guid())
                .Generate();
            var user = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, request.Id)
                .RuleFor(u => u.TenantId, _ => requestor.TenantId)
                .Generate();

            _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
            _userRepository.GetByUserId(request.Id).Returns(user);

            // Act
            await _userDeleteCommandHandler.Handle(request, cancellationToken);

            // Assert
            await _userRepository.Received(1).GetByUserId(request.RequestorId);
            await _userRepository.Received(1).GetByUserId(request.Id);
            await _userRepository.Received(1).Delete(user);
        }

        [Fact]
        public async Task Handle_Requestor_Is_Not_Admin_Throws_ForbiddenAccessException()
        {
            // Arrange
            var cancellationToken = default(CancellationToken);
            var request = AutoFaker.Generate<UserDeleteCommand>();
            var requestor = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.UserType, _ => UserType.USER)
                .Generate();

            _userRepository.GetByUserId(request.RequestorId).Returns(requestor);

            // Act
            var act = async () => await _userDeleteCommandHandler.Handle(request, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            await _userRepository.Received(1).GetByUserId(request.RequestorId);
            await _userRepository.DidNotReceive().GetByUserId(request.Id);
            await _userRepository.DidNotReceive().Delete(Arg.Any<Domain.Entities.User>());
        }

        [Fact]
        public async Task Handle_User_Not_Found_Throws_EntityNotFoundException()
        {
            // Arrange
            var cancellationToken = default(CancellationToken);
            var request = AutoFaker.Generate<UserDeleteCommand>();
            var requestor = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.UserType, _ => UserType.ADMIN)
                .Generate();

            _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
            _userRepository.GetByUserId(request.Id).Returns((Domain.Entities.User?)null);

            // Act
            var act = async () => await _userDeleteCommandHandler.Handle(request, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
            await _userRepository.Received(1).GetByUserId(request.RequestorId);
            await _userRepository.Received(1).GetByUserId(request.Id);
            await _userRepository.DidNotReceive().Delete(Arg.Any<Domain.Entities.User>());
        }

        [Fact]
        public async Task Handle_Different_TenantId_Throws_ForbiddenAccessException()
        {
            // Arrange
            var cancellationToken = default(CancellationToken);
            var request = AutoFaker.Generate<UserDeleteCommand>();
            var requestor = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.UserType, _ => UserType.ADMIN)
                .RuleFor(u => u.TenantId, f => f.Random.Guid())
                .Generate();
            var user = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, request.Id)
                .RuleFor(u => u.TenantId, f => f.Random.Guid())
                .Generate();

            _userRepository.GetByUserId(request.RequestorId).Returns(requestor);
            _userRepository.GetByUserId(request.Id).Returns(user);

            // Act
            var act = async () => await _userDeleteCommandHandler.Handle(request, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<ForbiddenAccessException>();
            await _userRepository.Received(1).GetByUserId(request.RequestorId);
            await _userRepository.Received(1).GetByUserId(request.Id);
            await _userRepository.DidNotReceive().Delete(Arg.Any<Domain.Entities.User>());
        }
    }
}
