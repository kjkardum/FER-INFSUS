using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Create;

public class UserRegistrationCommandHandler(
    IUserRepository userRepository,
    ISignInService signInService)
    : IRequestHandler<UserRegistrationCommand>
{
    public async Task Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);

        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Only admins can create users");
            throw new ForbiddenAccessException("Samo admini mogu kreirati korisnike");
        }

        if (await userRepository.DoesUserExist(request.NormalizedEmail))
        {
            throw new EntityAlreadyExistsException("Korisnik s ovim emailom već postoji");
        }

        var passwordHash = signInService.HashPassword(request.Password);
        if (passwordHash == null)
        {
            throw new BadRequestException("Nije moguće procesirati lozinku");
        }

        var user = new Domain.Entities.User {
            TenantId = requestor.TenantId,
            Email = request.NormalizedEmail,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            LastLogin = DateTime.UtcNow,
            UserType = request.UserType,
            DateOfBirth = request.DateOfBirth
        };

        await userRepository.Create(user);
    }
}
