using FluentValidation;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Update;

public class UserUpdateCommandValidator: AbstractValidator<UserUpdateCommand>
{
    public UserUpdateCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .MinimumLength(8)
            .MaximumLength(50)
            .When(t => t.NewPassword != null);
    }
}
