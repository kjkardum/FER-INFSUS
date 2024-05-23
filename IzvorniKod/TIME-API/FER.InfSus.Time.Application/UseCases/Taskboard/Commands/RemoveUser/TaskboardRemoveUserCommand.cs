using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.RemoveUser;

public class TaskboardRemoveUserCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    public Guid TaskboardId { get; set; }
    public Guid UserId { get; set; }
}
