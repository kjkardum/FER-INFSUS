using FluentValidation;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Update;

public class TaskboardUpdateCommandValidator: AbstractValidator<TaskboardUpdateCommand>
{
    public TaskboardUpdateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
