using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Assign;

public class TaskItemAssignCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository) : IRequestHandler<TaskItemAssignCommand>
{
    public async Task Handle(TaskItemAssignCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskItem = await taskItemRepository.GetById(request.Id, cancellationToken);
        if (taskItem == null)
        {
            throw new EntityNotFoundException("Task item not found");
        }
        if (taskItem.AssignedUserId == request.AssignedUserId)
        {
            return;
        }
        if (taskItem.Taskboard?.TenantId != requestor?.TenantId)
        {
            throw new ForbiddenAccessException(
                "You can't assign task items for taskboards that are not in your tenant");
        }
        if (requestor?.UserType != UserType.ADMIN
            && taskItem.Taskboard!.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException(
                "You can't assign task items for taskboards you are not a member of");
        }
        if (request.AssignedUserId != null
            && taskItem.Taskboard!.TaskboardUsers!.All(tu => tu.UserId != request.AssignedUserId))
        {
            throw new ForbiddenAccessException(
                "You can't assign task items to users who are not members of the taskboard");
        }

        var now = DateTime.UtcNow;
        TaskItemHistoryLog historyLog;
        if (request.AssignedUserId != null)
        {
            var assignedUser = await userRepository.GetByUserId((Guid)request.AssignedUserId!);
            historyLog = new TaskItemHistoryLog
            {
                TaskItemId = taskItem.Id,
                Changelog = $"""
                             {requestor!.FirstName} {requestor!.LastName} dodijelio zadatak
                             Dodijelio: {assignedUser!.FirstName} {assignedUser!.LastName}
                             Prethodno dodijeljen: {taskItem.AssignedUser?.FirstName ?? string.Empty} {taskItem.AssignedUser?.LastName ?? string.Empty}
                             """,
                ModifiedAt = now,
            };
        }
        else
        {
            historyLog = new TaskItemHistoryLog
            {
                TaskItemId = taskItem.Id,
                Changelog = $"""
                             {requestor!.FirstName} {requestor!.LastName} uklonio dodijeljenost zadatka
                             Prethodno dodijeljen: {taskItem.AssignedUser?.FirstName ?? string.Empty} {taskItem.AssignedUser?.LastName ?? string.Empty}
                             """,
                ModifiedAt = now,
            };
        }

        taskItem.AssignedUserId = request.AssignedUserId;
        await taskItemRepository.Update(taskItem, cancellationToken);
        await taskItemRepository.AddHistoryLog(historyLog, cancellationToken);
    }
}
