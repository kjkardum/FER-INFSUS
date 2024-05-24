using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Application.Repositories;

public interface ITaskItemRepository
{
    Task Create(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task Update(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task Delete(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetById(Guid id, CancellationToken cancellationToken = default);
    Task AddHistoryLog(TaskItemHistoryLog historyLog, CancellationToken cancellationToken = default);
}
