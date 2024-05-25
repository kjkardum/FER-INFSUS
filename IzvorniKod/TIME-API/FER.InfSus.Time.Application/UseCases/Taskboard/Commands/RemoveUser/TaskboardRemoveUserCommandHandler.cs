using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.RemoveUser;

public class TaskboardRemoveUserCommandHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository) : IRequestHandler<TaskboardRemoveUserCommand>
{
    public async Task Handle(TaskboardRemoveUserCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Samo admini mogu uklanjati korisnike s radnih ploča");
        }
        var taskboard = await taskboardRepository.GetBoardById(request.TaskboardId);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Radna ploča nije pronađena");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("Nemate dozvolu za uklanjanje korisnika s ove radne ploče");
        }
        if (taskboard.TaskboardUsers!.All(u => u.UserId != request.UserId))
        {
            throw new EntityNotFoundException("Korisnik nema pristup radnoj ploči");
        }
        await taskboardRepository.RemoveUserFromBoard(request.TaskboardId, request.UserId);
    }
}
