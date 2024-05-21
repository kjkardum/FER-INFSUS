using FER.InfSus.Time.Domain.Enums;

namespace FER.InfSus.Time.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime LastLogin { get; set; }
    public UserType UserType { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt  { get; set; }
    public ICollection<UserTaskboardAssociation>? UserTaskboards { get; set; }
    public ICollection<TaskItem>? TaskItems { get; set; }
    public ICollection<TimesheetEntry>? TimesheetEntries { get; set; }
}
