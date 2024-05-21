namespace FER.InfSus.Time.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public Guid TaskboardId { get; set; }
    public Taskboard? Taskboard { get; set; }
    public Guid? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<TaskItemHistoryLog>? HistoryLogs { get; set; }
}
