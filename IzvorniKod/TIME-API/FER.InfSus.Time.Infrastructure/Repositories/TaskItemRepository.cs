using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FER.InfSus.Time.Infrastructure.Repositories;

public class TaskItemRepository(ApplicationDbContext dbContext): ITaskItemRepository
{
    public async Task Create(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(taskItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        dbContext.Update(taskItem);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        dbContext.Remove(taskItem);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetById(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.TaskItems
            .Include(t => t.Taskboard)
            .ThenInclude(tb => tb.TaskboardUsers)
            .Include(t => t.AssignedUser)
            .Include(t => t.HistoryLogs)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddHistoryLog(TaskItemHistoryLog historyLog, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(historyLog, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ICollection<TaskItem>> GetByAssignedUserId(Guid assignedUserId) =>
        await dbContext.TaskItems
            .Include(t => t.AssignedUser)
            .Include(t => t.Taskboard)
            .Where(t => t.AssignedUserId == assignedUserId)
            .ToListAsync();
}
