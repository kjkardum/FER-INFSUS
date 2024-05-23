using ApiExceptions.Exceptions;
using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Create;

public class TaskboardCreateCommandHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<TaskboardCreateCommand, TaskboardSimpleDto>
{
    /// <summary>
    /// Handle create taskboard command
    /// </summary>
    /// <param name="request">param</param>
    /// <param name="cancellationToken">param</param>
    /// <exception cref="ForbiddenAccessException"></exception>
    public async Task<TaskboardSimpleDto> Handle(TaskboardCreateCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByUserId(request.RequestorId);
        if (user?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Only admins can create taskboards");
        }

        var taskboard = new Domain.Entities.Taskboard
        {
            Name = request.Name,
            Description = request.Description,
            TenantId = user.TenantId,
        };

        await taskboardRepository.CreateBoard(taskboard);

        var mappedTaskboard = mapper.Map<TaskboardSimpleDto>(taskboard);
        return mappedTaskboard;
    }
}
