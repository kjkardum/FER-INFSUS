using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Update;

public class TaskboardUpdateCommand: TaskboardInputDto, IRequest;
