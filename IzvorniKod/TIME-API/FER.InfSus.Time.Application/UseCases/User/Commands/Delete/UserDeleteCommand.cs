using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Delete;

public class UserDeleteCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    [JsonIgnore] public Guid Id { get; set; }
}
