using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeTaskboard;

public class TaskItemChangeTaskboardCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    [JsonIgnore] public Guid Id { get; set; }
    public required Guid NewTaskboardId { get; set; }
}
