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
            throw new EntityNotFoundException("Task item not found");
        }
        if (taskItem.TaskboardId == request.NewTaskboardId)
        {
            return;
        }
        var taskboard = await taskboardRepository.GetBoardById(request.NewTaskboardId);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("New taskboard not found");
        }
        if (taskboard.TenantId != requestor?.TenantId || taskItem.Taskboard!.TenantId != requestor.TenantId)

        {
            throw new ForbiddenAccessException(
                "You can't change taskboard of task items to taskboards that are not in your tenant");
        }
        if (requestor.UserType != UserType.ADMIN
            && taskboard.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException(
                "You can't change taskboard of task items to taskboards you are not a member of");
        }
        if (taskItem.AssignedUserId != null
            && taskboard.TaskboardUsers!.All(tu => tu.UserId != taskItem.AssignedUserId))
        {
            throw new ForbiddenAccessException(
                "You can't change taskboard of task items that are assigned to users that are not members of the new taskboard");
        }

        var now = DateTime.UtcNow;
        var historyLog = new TaskItemHistoryLog
        {
            TaskItemId = taskItem.Id,
            Changelog = $"""
                         {requestor!.FirstName} {requestor!.LastName} promijenio radnu ploču zadatka
                         Stara radna ploča: {taskItem.Taskboard!.Name}
                         Nova radna ploča: {taskboard.Name}
                         """,
            ModifiedAt = now,
        };

        taskItem.TaskboardId = request.NewTaskboardId;
        await taskItemRepository.Update(taskItem, cancellationToken);
        await taskItemRepository.AddHistoryLog(historyLog, cancellationToken);
    }
}
