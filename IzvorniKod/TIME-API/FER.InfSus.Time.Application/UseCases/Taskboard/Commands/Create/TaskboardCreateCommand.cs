using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Create;

public class TaskboardCreateCommand : TaskboardInputDto, IRequest<TaskboardSimpleDto>;
