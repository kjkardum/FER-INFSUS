using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByUserId;

public class TaskboardGetAllByUserIdQuery: IRequest<ICollection<TaskboardSimpleDto>>
{
    public Guid RequestorId { get; set; }
}
