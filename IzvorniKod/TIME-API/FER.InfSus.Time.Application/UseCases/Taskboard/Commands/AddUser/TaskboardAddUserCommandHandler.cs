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
            throw new ForbiddenAccessException("Only admins can add users to taskboards");
        }
        var taskboard = await taskboardRepository.GetBoardById(request.TaskboardId);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Taskboard not found");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("You don't have permission to add users to this taskboard");
        }
        if (taskboard.TaskboardUsers!.Any(u => u.UserId == request.UserId))
        {
            throw new EntityAlreadyExistsException("User is already in the taskboard");
        }

        await taskboardRepository.AddUserToBoard(request.TaskboardId, request.UserId);
    }
}
