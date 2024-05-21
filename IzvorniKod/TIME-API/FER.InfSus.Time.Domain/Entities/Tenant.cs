namespace FER.InfSus.Time.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public ICollection<User>? Users { get; set; }
    public ICollection<Report>? Reports { get; set; }
    public ICollection<Taskboard>? Taskboards { get; set; }
}
