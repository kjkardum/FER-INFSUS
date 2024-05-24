using FER.InfSus.Time.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeState;

public class TaskItemChangeStateCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    [JsonIgnore] public Guid Id { get; set; }
    public required TaskItemState NewState { get; set; }
}
