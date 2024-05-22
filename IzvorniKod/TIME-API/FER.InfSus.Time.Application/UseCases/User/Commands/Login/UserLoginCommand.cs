using MediatR;
using FER.InfSus.Time.Application.UseCases.User.Dto;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Login;

public record UserLoginCommand: IRequest<LoggedInUserDto>
{
    public string Email { get; set; } = null!;

    public string NormalizedEmail
        => Email.Trim().ToUpperInvariant();
    public string Password { get; set; } = null!;
}
