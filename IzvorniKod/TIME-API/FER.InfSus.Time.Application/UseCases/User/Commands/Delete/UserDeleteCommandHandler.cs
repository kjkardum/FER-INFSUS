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
            throw new ForbiddenAccessException("Only admins can delete users");
        }
        var user = await userRepository.GetByUserId(request.Id);
        if (user == null)
        {
            throw new EntityNotFoundException("User not found");
        }
        if (user.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("You can't delete users that are not in your tenant");
        }

        await userRepository.Delete(user);
    }
}
