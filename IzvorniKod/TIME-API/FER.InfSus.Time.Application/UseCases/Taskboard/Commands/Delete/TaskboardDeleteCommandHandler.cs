using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Delete;

public class TaskboardDeleteCommandHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository) : IRequestHandler<TaskboardDeleteCommand>
{
    /// <summary>
    /// Handle delete taskboard command
    /// </summary>
    /// <param name="request">param</param>
    /// <param name="cancellationToken">param</param>
    /// <exception cref="ForbiddenAccessException"></exception>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task Handle(TaskboardDeleteCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Only admins can delete taskboards");
        }

        var taskboard = await taskboardRepository.GetBoardById(request.TaskboardId);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Taskboard not found");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("You don't have permission to delete this taskboard");
        }
        await taskboardRepository.DeleteBoard(taskboard);
    }
}
