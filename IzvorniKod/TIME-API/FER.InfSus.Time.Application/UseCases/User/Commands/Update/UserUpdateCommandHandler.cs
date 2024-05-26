using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Update;

public class UserUpdateCommandHandler(
    IUserRepository userRepository,
    ISignInService signInService) : IRequestHandler<UserUpdateCommand>
{
    public async Task Handle(UserUpdateCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Samo admini mogu ažurirati podatke korisnika");
        }
        var user = await userRepository.GetByUserId(request.Id);
        if (user == null)
        {
            throw new EntityNotFoundException("Korisnik nije pronađen");
        }
        if (user.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za ažuriranje podataka korisnika koji nisu u vašoj organizaciji");
        }
        if (request.NewPassword != null)
        {
            var passwordHash = signInService.HashPassword(request.NewPassword);
            if (passwordHash == null)
            {
                throw new BadRequestException("Nije moguće procesirati lozinku");
            }
            user.PasswordHash = passwordHash;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DateOfBirth = request.DateOfBirth;
        user.UserType = request.UserType;

        await userRepository.Update(user);
    }
}
