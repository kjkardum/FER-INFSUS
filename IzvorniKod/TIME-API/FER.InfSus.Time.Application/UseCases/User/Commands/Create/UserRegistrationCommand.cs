using MediatR;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Create;

public record UserRegistrationCommand: IRequest
{
    public Guid RequestorId { get; set; }
    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string NormalizedEmail
        => Email.Trim().ToUpperInvariant();
    public string Password { get; set; } = null!;
}
