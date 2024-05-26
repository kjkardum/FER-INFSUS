using FER.InfSus.Time.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Update;

public class UserUpdateCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    [JsonIgnore] public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string? NewPassword { get; set; }
    public UserType UserType { get; set; }
}
