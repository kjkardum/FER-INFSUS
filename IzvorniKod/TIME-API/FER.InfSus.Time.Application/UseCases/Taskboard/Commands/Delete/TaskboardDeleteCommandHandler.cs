using ApiExceptions.Exceptions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Delete;

public class TaskboardDeleteCommandHandler(
    ITaskboardRepository taskboardRepository,
    IUserRepository userRepository) : IRequestHandler<TaskboardDeleteCommand>
{
    /// <summary>
    /// Handle delete taskboard command
    /// </summary>
    /// <param name="request">param</param>
    /// <param name="cancellationToken">param</param>
    /// <exception cref="ForbiddenAccessException"></exception>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task Handle(TaskboardDeleteCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Sammo admini mogu brisati radne ploče");
        }

        var taskboard = await taskboardRepository.GetBoardById(request.TaskboardId);
        if (taskboard == null)
        {
            throw new EntityNotFoundException("Radna ploča nije pronađena");
        }
        if (taskboard.TenantId != requestor.TenantId)
        {
            throw new ForbiddenAccessException("Nemate dozvolu za brisanje ove radne ploče");
        }
        await taskboardRepository.DeleteBoard(taskboard);
    }
}
