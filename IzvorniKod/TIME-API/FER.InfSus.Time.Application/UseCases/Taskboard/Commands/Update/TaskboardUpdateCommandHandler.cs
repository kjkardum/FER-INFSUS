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
            throw new ForbiddenAccessException("Samo admini mogu ažurirati radne ploče");
        }
        var taskboard = await taskboardRepository.GetBoardById(request.Id);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Radna ploča nije pronađena");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("Nemate dozvolu za ažuriranje ove radne ploče");
        }
        taskboard.Name = request.Name;
        taskboard.Description = request.Description;
        await taskboardRepository.UpdateBoard(taskboard);
    }
}
