namespace FER.InfSus.Time.Domain.Entities;

public class Taskboard
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public ICollection<TaskItem>? TaskItems { get; set; }
    public ICollection<UserTaskboardAssociation>? TaskboardUsers { get; set; }
}
