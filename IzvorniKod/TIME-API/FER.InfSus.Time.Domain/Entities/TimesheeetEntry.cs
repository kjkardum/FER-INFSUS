namespace FER.InfSus.Time.Domain.Entities;

public class TimesheetEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateOnly Date { get; set; }
    public int Hours { get; set; }
    public bool RemoteWork { get; set; }
    public Guid? TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
}
