using FluentValidation;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Login;

public class UserLoginCommandValidator: AbstractValidator<UserLoginCommand>
{
    public UserLoginCommandValidator()
    {
        RuleFor(t => t.Email)
            .EmailAddress();
        RuleFor(t => t.Password)
            .NotEmpty();
    }
}
