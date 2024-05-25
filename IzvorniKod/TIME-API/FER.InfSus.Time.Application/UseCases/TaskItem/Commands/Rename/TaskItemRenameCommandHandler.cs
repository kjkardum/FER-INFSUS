using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Rename;

public class TaskItemRenameCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository): IRequestHandler<TaskItemRenameCommand>
{
    public async Task Handle(TaskItemRenameCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskItem = await taskItemRepository.GetById(request.Id, cancellationToken);
        if (taskItem == null)
        {
            throw new EntityNotFoundException("Zadatak nije pronađen");
        }
        if (taskItem.Name == request.NewName)
        {
            return;
        }
        if (taskItem.Taskboard!.TenantId != requestor?.TenantId)
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu naziva zadatka sa radne ploče koje nisu u vašoj organizaciji");
        }
        if (requestor.UserType != UserType.ADMIN
            && taskItem.Taskboard!.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu naziva zadatka sa radne ploče na kojoj niste član");
        }

        var now = DateTime.UtcNow;
        var historyLog = new TaskItemHistoryLog
        {
            TaskItemId = taskItem.Id,
            Changelog = $"""
                         {requestor!.FirstName} {requestor!.LastName} promijenio naziv zadatka
                         Stari naziv: {taskItem.Name}
                         Novi naziv: {request.NewName}
                         """,
            ModifiedAt = now,
        };

        taskItem.Name = request.NewName;
        await taskItemRepository.Update(taskItem, cancellationToken);
        await taskItemRepository.AddHistoryLog(historyLog, cancellationToken);
    }
}
