using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Delete;

public class TaskItemDeleteCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository) : IRequestHandler<TaskItemDeleteCommand>
{
    public async Task Handle(TaskItemDeleteCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskItem = await taskItemRepository.GetById(request.Id, cancellationToken);
        if (taskItem == null)
        {
            throw new EntityNotFoundException("Task item not found");
        }
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("You can't delete task items");
        }
        if (taskItem.Taskboard?.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("You can't delete task items from taskboards you are not a member of");
        }
        await taskItemRepository.Delete(taskItem, cancellationToken);
    }
}
