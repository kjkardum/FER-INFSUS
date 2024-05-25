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
            throw new EntityNotFoundException("Zadatak nije pronađen");
        }
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Samo admini mogu brisati zadatke");
        }
        if (taskItem.Taskboard?.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za brisanje zadataka sa radnih ploča koje nisu u vašoj organizaciji");
        }
        await taskItemRepository.Delete(taskItem, cancellationToken);
    }
}
