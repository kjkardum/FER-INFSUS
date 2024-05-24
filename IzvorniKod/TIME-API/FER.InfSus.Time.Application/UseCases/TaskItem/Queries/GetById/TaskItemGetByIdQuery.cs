using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetById;

public class TaskItemGetByIdQuery: IRequest<TaskItemDetailedDto>
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    [JsonIgnore] public Guid Id { get; set; }
}
