using ApiExceptions.Exceptions;
using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByTenant;

public class TaskboardGetAllByTenantQueryHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<TaskboardGetAllByTenantQuery, ICollection<TaskboardSimpleDto>>
{
    public async Task<ICollection<TaskboardSimpleDto>> Handle(
        TaskboardGetAllByTenantQuery request,
        CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Only admins can get taskboards");
        }
        var taskboards = await taskboardRepository.GetTenantBoards(requestor.TenantId);
        var mappedTaskboards = mapper.Map<ICollection<TaskboardSimpleDto>>(taskboards);
        return mappedTaskboards;
    }
}
