using ApiExceptions.Exceptions;
using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetById;

public class TaskboardGetByIdQueryHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<TaskboardGetByIdQuery, TaskboardDetailedDto>
{
    public async Task<TaskboardDetailedDto> Handle(TaskboardGetByIdQuery request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        var taskboard = await taskboardRepository.GetBoardById(request.Id);
        if (taskboard == null || requestor == null)
        {
            throw new EntityNotFoundException("Taskboard not found");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("You don't have permission to get this taskboard");
        }
        if (taskboard.TaskboardUsers!.All(tu => tu.UserId != request.RequestorId))
        {
            throw new ForbiddenAccessException("You don't have permission to get this taskboard");
        }
        var mappedTaskboard = mapper.Map<TaskboardDetailedDto>(taskboard);
        return mappedTaskboard;
    }
}
