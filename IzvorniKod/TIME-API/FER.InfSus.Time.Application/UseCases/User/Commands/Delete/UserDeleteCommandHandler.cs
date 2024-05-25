using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Delete;

public class UserDeleteCommandHandler(IUserRepository userRepository) : IRequestHandler<UserDeleteCommand>
{
    public async Task Handle(UserDeleteCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Samo admini mogu brisati korisnike");
        }
        var user = await userRepository.GetByUserId(request.Id);
        if (user == null)
        {
            throw new EntityNotFoundException("Korisnik nije pronađen");
        }
        if (user.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("Nemate dozvolu za brisanje korisnika koji nisu u vašoj organizaciji");
        }

        await userRepository.Delete(user);
    }
}
