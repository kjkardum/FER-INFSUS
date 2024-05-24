namespace FER.InfSus.Time.Domain.Entities;

public class TaskItemHistoryLog
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public required string Changelog { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
