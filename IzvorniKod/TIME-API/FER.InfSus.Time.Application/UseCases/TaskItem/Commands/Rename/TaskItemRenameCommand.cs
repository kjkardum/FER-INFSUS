using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Rename;

public class TaskItemRenameCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    [JsonIgnore] public Guid Id { get; set; }
    public required string NewName { get; set; }
}
