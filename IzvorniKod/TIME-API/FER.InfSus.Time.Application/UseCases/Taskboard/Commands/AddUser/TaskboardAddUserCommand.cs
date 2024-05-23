using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.AddUser;

public class TaskboardAddUserCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    public Guid TaskboardId { get; set; }
    public Guid UserId { get; set; }
}
