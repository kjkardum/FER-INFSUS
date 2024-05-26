namespace FER.InfSus.Time.Domain.Entities;

public class UserTaskboardAssociation
{
    public Guid Id { get; set; }
    public Guid TaskboardId { get; set; }
    public Taskboard? Taskboard { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
