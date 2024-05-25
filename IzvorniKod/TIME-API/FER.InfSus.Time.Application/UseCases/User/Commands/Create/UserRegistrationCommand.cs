using FER.InfSus.Time.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Create;

public record UserRegistrationCommand: IRequest
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public UserType UserType { get; set; } = UserType.USER;

    public string NormalizedEmail
        => Email.Trim().ToUpperInvariant();
    public string Password { get; set; } = null!;
}
