using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.AddUser;

public class TaskboardAddUserCommandHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository) : IRequestHandler<TaskboardAddUserCommand>
{
    /// <summary>
    /// Add user to taskboard
    /// </summary>
    /// <param name="request">param</param>
    /// <param name="cancellationToken">param</param>
    /// <exception cref="ForbiddenAccessException"></exception>
    /// <exception cref="EntityNotFoundException"></exception>
    /// <exception cref="EntityAlreadyExistsException"></exception>
    public async Task Handle(TaskboardAddUserCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Samo admini mogu dodavati korisnike na radnu ploču");
        }
        var taskboard = await taskboardRepository.GetBoardById(request.TaskboardId);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Radna ploča nije pronađena");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("Nemate dozvolu za dodavanje korisnika na ovu radnu ploču");
        }
        if (taskboard.TaskboardUsers!.Any(u => u.UserId == request.UserId))
        {
            throw new EntityAlreadyExistsException("Korisnik je već dodan na radnu ploču");
        }

        await taskboardRepository.AddUserToBoard(request.TaskboardId, request.UserId);
    }
}
