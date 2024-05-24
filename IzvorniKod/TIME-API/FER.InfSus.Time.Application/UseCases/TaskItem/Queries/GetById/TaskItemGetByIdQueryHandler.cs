using ApiExceptions.Exceptions;
using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetById;

public class TaskItemGetByIdQueryHandler(
    IUserRepository userRepository,
    ITaskItemRepository taskItemRepository,
    IMapper mapper) : IRequestHandler<TaskItemGetByIdQuery, TaskItemDetailedDto>
{
    public async Task<TaskItemDetailedDto> Handle(TaskItemGetByIdQuery request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskItem = await taskItemRepository.GetById(request.Id, cancellationToken);
        if (taskItem == null)
        {
            throw new EntityNotFoundException("Task item not found");
        }
        if (taskItem.Taskboard!.TenantId != requestor?.TenantId)
        {
            throw new ForbiddenAccessException(
                "You can't view task items for taskboards that are not in your tenant");
        }
        if (requestor.UserType != UserType.ADMIN
            && taskItem.Taskboard!.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException("You can't view task items for taskboards you are not a member of");
        }

        return mapper.Map<TaskItemDetailedDto>(taskItem);
    }
}
