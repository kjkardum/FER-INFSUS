using ApiExceptions.Exceptions;
using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Create;

public class TaskItemCreateCommandHandler(
    ITaskItemRepository taskItemRepository,
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<TaskItemCreateCommand, TaskItemSimpleDto>
{
    public async Task<TaskItemSimpleDto> Handle(TaskItemCreateCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskboard = await taskboardRepository.GetBoardById(request.TaskboardId);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Radna ploča nije pronađena");
        }
        if (taskboard.TenantId != requestor?.TenantId)
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za stvaranje zadataka za radne ploče koje nisu u vašoj organizaciji");
        }
        if (requestor.UserType != UserType.ADMIN
            && taskboard.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException(
                "Nemate dozvolu za stvaranje zadataka za radne ploče na kojima niste član");
        }

        var now = DateTime.UtcNow;
        var taskItem = new Domain.Entities.TaskItem
        {
            Name = request.Name,
            Description = request.Description,
            State = TaskItemState.Novo,
            TaskboardId = request.TaskboardId,
            CreatedAt = now,
            HistoryLogs = new List<TaskItemHistoryLog>
            {
                new()
                {
                    Changelog = $"""
                                {requestor!.FirstName} {requestor!.LastName} stvorio novi zadatak
                                Naziv: {request.Name}
                                Opis: {request.Description}
                                Stanje: {nameof(TaskItemState.Novo)}
                                Radna ploča: {taskboard.Name}
                                """,
                    ModifiedAt = now,
                }
            }
        };

        await taskItemRepository.Create(taskItem, cancellationToken);
        var mappedTaskItem = mapper.Map<TaskItemSimpleDto>(taskItem);
        return mappedTaskItem;
    }
}
