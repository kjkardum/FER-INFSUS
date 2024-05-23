using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Delete;

public class TaskboardDeleteCommand: IRequest
{
    public Guid RequestorId { get; set; }
    public Guid TaskboardId { get; set; }
}
