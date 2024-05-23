using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByTenant;

public class TaskboardGetAllByTenantQuery: IRequest<ICollection<TaskboardSimpleDto>>
{
    public Guid RequestorId { get; set; }
}
