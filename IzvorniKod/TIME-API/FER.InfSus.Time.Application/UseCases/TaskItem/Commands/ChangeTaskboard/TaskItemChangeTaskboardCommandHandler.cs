using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeTaskboard;

public class TaskItemChangeTaskboardCommandHandler(
    ITaskItemRepository taskItemRepository,
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository) : IRequestHandler<TaskItemChangeTaskboardCommand>
{
    public async Task Handle(TaskItemChangeTaskboardCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskItem = await taskItemRepository.GetById(request.Id, cancellationToken);
        if (taskItem == null)
        {
            throw new EntityNotFoundException("Zadatak nije pronađen");
        }
        if (taskItem.TaskboardId == request.NewTaskboardId)
        {
            return;
        }
        var newTaskboard = await taskboardRepository.GetBoardById(request.NewTaskboardId);
        if (newTaskboard == null)
        {
            throw new EntityNotFoundException("Nova radna ploča nije pronađena");
        }
        if (newTaskboard.TenantId != requestor?.TenantId || taskItem.Taskboard!.TenantId != requestor.TenantId)

        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu radne ploče zadatka na one koje nisu u vašoj organizaciji");
        }
        if (requestor.UserType != UserType.ADMIN
            && taskItem.Taskboard!.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu radne ploče zadatka sa one na kojoj niste član");
        }
        if (requestor.UserType != UserType.ADMIN
            && newTaskboard.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu radne ploče zadatka na one na kojima niste član");
        }
        if (taskItem.AssignedUserId != null
            && newTaskboard.TaskboardUsers!.All(tu => tu.UserId != taskItem.AssignedUserId))
        {
            throw new ForbiddenAccessException(
                "Ne možete promijeniti radnu ploču zadataka koji su dodijeljeni korisnicima koji nisu članovi nove radne ploče");
        }

        var now = DateTime.UtcNow;
        var historyLog = new TaskItemHistoryLog
        {
            TaskItemId = taskItem.Id,
            Changelog = $"""
                         {requestor!.FirstName} {requestor!.LastName} promijenio radnu ploču zadatka
                         Stara radna ploča: {taskItem.Taskboard!.Name}
                         Nova radna ploča: {newTaskboard.Name}
                         """,
            ModifiedAt = now,
        };

        taskItem.TaskboardId = request.NewTaskboardId;
        await taskItemRepository.Update(taskItem, cancellationToken);
        await taskItemRepository.AddHistoryLog(historyLog, cancellationToken);
    }
}
