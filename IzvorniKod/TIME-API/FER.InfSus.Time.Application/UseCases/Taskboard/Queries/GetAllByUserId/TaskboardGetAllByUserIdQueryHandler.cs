using ApiExceptions.Exceptions;
using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByUserId;

public class TaskboardGetAllByUserIdQueryHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<TaskboardGetAllByUserIdQuery, ICollection<TaskboardSimpleDto>>
{
    public async Task<ICollection<TaskboardSimpleDto>> Handle(
        TaskboardGetAllByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var taskboards = await taskboardRepository.GetBoardsByUserId(request.RequestorId);

        var mappedTaskboards = mapper.Map<ICollection<TaskboardSimpleDto>>(taskboards);
        return mappedTaskboards;
    }
}
