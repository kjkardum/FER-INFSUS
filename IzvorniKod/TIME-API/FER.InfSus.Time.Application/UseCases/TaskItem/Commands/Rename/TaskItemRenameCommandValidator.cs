using FluentValidation;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Rename;

public class TaskItemRenameCommandValidator: AbstractValidator<TaskItemRenameCommand>
{
    public TaskItemRenameCommandValidator()
    {
        RuleFor(x => x.NewName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
