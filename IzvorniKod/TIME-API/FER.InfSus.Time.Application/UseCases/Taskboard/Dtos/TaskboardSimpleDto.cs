using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Application.UseCases.User.Dto;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;

public class TaskboardSimpleDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}