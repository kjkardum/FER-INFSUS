using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Create;

public class TaskItemCreateCommand: IRequest<TaskItemSimpleDto>
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public Guid TaskboardId { get; set; }
}
