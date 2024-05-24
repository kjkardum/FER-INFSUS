using FluentValidation;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Create;

public class TaskItemCreateCommandValidator: AbstractValidator<TaskItemCreateCommand>
{
    public TaskItemCreateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.TaskboardId).NotEmpty();
    }
}
