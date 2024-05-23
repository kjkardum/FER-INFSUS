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
            throw new ForbiddenAccessException("Only admins can remove users from taskboards");
        }
        var taskboard = await taskboardRepository.GetBoardById(request.TaskboardId);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Taskboard not found");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("You don't have permission to remove users from this taskboard");
        }
        if (taskboard.TaskboardUsers!.All(u => u.UserId != request.UserId))
        {
            throw new EntityNotFoundException("User is not in the taskboard");
        }
        await taskboardRepository.RemoveUserFromBoard(request.TaskboardId, request.UserId);
    }
}
