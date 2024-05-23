using FluentValidation;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Create;

public class TaskboardCreateCommandValidator: AbstractValidator<TaskboardCreateCommand>
{
    public TaskboardCreateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
