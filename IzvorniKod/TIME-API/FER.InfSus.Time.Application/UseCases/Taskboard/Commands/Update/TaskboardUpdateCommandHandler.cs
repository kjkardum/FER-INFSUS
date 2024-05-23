using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Update;

public class TaskboardUpdateCommandHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository) : IRequestHandler<TaskboardUpdateCommand>
{
    public async Task Handle(TaskboardUpdateCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Only admins can update taskboards");
        }
        var taskboard = await taskboardRepository.GetBoardById(request.Id);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Taskboard not found");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("You don't have permission to update this taskboard");
        }
        taskboard.Name = request.Name;
        taskboard.Description = request.Description;
        await taskboardRepository.UpdateBoard(taskboard);
    }
}
