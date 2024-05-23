using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetById;

public class TaskboardGetByIdQuery: IRequest<TaskboardDetailedDto>
{
    public Guid RequestorId { get; set; }
    public Guid Id { get; set; }
}
