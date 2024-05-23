using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;

public class TaskboardInputDto
{
    [JsonIgnore] public Guid RequestorId { get; set; }
    [JsonIgnore] public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}
