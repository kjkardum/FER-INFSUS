namespace FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;

public class TaskItemHistoryLogDto
{
    public required string Changelog { get; set; }
    public required DateTime ModifiedAt { get; set; }
}
