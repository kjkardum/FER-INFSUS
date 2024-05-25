using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeState;

public class TaskItemChangeStateCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository) : IRequestHandler<TaskItemChangeStateCommand>
{
    public async Task Handle(TaskItemChangeStateCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskItem = await taskItemRepository.GetById(request.Id, cancellationToken);
        if (taskItem == null)
        {
            throw new EntityNotFoundException("Zadatak nije pronađen");
        }
        if (taskItem.State == request.NewState)
        {
            return;
        }
        if (taskItem.Taskboard?.TenantId != requestor?.TenantId)
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu stanja zadatka za radne ploče koje nisu u vašoj organizaciji");
        }
        if (requestor?.UserType != UserType.ADMIN
            && taskItem.Taskboard!.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu stanja zadatka za radne ploče na kojima niste član");
        }

        var now = DateTime.UtcNow;
        var historyLog = new TaskItemHistoryLog
        {
            TaskItemId = taskItem.Id,
            Changelog = $"""
                         {requestor!.FirstName} {requestor!.LastName} promijenio stanje zadatka
                         Staro stanje: {taskItem.State}
                         Novo stanje: {request.NewState}
                         """,
            ModifiedAt = now,
        };

        taskItem.State = request.NewState;
        await taskItemRepository.Update(taskItem, cancellationToken);
        await taskItemRepository.AddHistoryLog(historyLog, cancellationToken);
    }
}
