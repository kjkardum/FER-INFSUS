namespace FER.InfSus.Time.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public DateTime TimeOfGenerating { get; set; }
    public required string Path { get; set; }
}
