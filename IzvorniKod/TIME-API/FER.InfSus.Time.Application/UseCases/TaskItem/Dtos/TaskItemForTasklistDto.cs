using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Domain.Enums;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;

public class TaskItemForTasklistDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public TaskItemState State { get; set; }
    public Guid TaskboardId { get; set; }
    public required string TaskboardName { get; set; }
    public Guid? AssignedUserId { get; set; }
    public UserDto? AssignedUser { get; set; }
    public DateTime CreatedAt { get; set; }
}
