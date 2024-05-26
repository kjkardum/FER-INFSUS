using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetAllByAssignedUser;

public class TaskItemGetAllByAssignedUserQueryHandler(
    ITaskItemRepository taskItemRepository,
    IMapper mapper) : IRequestHandler<TaskItemGetAllByAssignedUserQuery, ICollection<TaskItemSimpleDto>>
{
    public async Task<ICollection<TaskItemSimpleDto>> Handle(
        TaskItemGetAllByAssignedUserQuery request,
        CancellationToken cancellationToken)
    {
        var taskItems = await taskItemRepository.GetByAssignedUserId(request.RequestorId);

        var mappedTaskItems = mapper.Map<ICollection<TaskItemSimpleDto>>(taskItems);
        return mappedTaskItems;
    }
}
