using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeDescription;

public class TaskItemChangeDescriptionCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository) : IRequestHandler<TaskItemChangeDescriptionCommand>
{
    public async Task Handle(TaskItemChangeDescriptionCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskItem = await taskItemRepository.GetById(request.Id, cancellationToken);
        if (taskItem == null)
        {
            throw new EntityNotFoundException("Zadatak nije pronađen");
        }
        if (taskItem.Description == request.NewDescription)
        {
            return;
        }
        if (taskItem.Taskboard?.TenantId != requestor?.TenantId)
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu opisa zadatka za radne ploče koje nisu u vašoj organizaciji");
        }
        if (requestor?.UserType != UserType.ADMIN
            && taskItem.Taskboard!.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za promjenu opisa zadatka za radne ploče na kojima niste član");
        }

        var now = DateTime.UtcNow;
        var historyLog = new TaskItemHistoryLog
        {
            TaskItemId = taskItem.Id,
            Changelog = $"""
                         {requestor!.FirstName} {requestor!.LastName} promijenio opis zadatka
                         Stari opis: {taskItem.Description}
                         Novi opis: {request.NewDescription}
                         """,
            ModifiedAt = now,
        };

        taskItem.Description = request.NewDescription;
        await taskItemRepository.Update(taskItem, cancellationToken);
        await taskItemRepository.AddHistoryLog(historyLog, cancellationToken);
    }
}
