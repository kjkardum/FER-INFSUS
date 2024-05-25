using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Update;

public class UserUpdateCommandHandler(
    IUserRepository userRepository) : IRequestHandler<UserUpdateCommand>
{
    public async Task Handle(UserUpdateCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Only admins can update user data");
        }
        var user = await userRepository.GetByUserId(request.Id);
        if (user == null)
        {
            throw new EntityNotFoundException("User not found");
        }
        if (user.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("You can't update user data of users that are not in your tenant");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.DateOfBirth = request.DateOfBirth;

        await userRepository.Update(user);
    }
}
