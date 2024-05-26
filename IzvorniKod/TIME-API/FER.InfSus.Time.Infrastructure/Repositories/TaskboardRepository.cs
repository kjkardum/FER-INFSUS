using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FER.InfSus.Time.Infrastructure.Repositories;

public class TaskboardRepository(ApplicationDbContext dbContext) : ITaskboardRepository
{
    public async Task CreateBoard(Taskboard board)
    {
        await dbContext.AddAsync(board);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateBoard(Taskboard board)
    {
        dbContext.Update(board);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Taskboard?> GetBoardById(Guid boardId)
        => await dbContext.Taskboards
            .Include(t => t.TaskItems!)
            .ThenInclude(t => t.AssignedUser)
            .Include(t => t.TaskboardUsers!)
            .ThenInclude(tu => tu.User)
            .FirstOrDefaultAsync(b => b.Id == boardId);

    public async Task AddUserToBoard(Guid boardId, Guid userId)
    {
        var userTaskboardAssociation = new UserTaskboardAssociation
        {
            TaskboardId = boardId,
            UserId = userId
        };

        await dbContext.AddAsync(userTaskboardAssociation);
        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveUserFromBoard(Guid boardId, Guid userId)
    {
        var userTaskboardAssociation = await dbContext.UserTaskboardAssociations
            .FirstOrDefaultAsync(uta => uta.TaskboardId == boardId && uta.UserId == userId);

        if (userTaskboardAssociation is not null)
        {
            dbContext.Remove(userTaskboardAssociation);
        }

        var userTasksOnBoard = await dbContext.TaskItems
            .Where(t => t.TaskboardId == boardId && t.AssignedUserId == userId)
            .ToListAsync();

        foreach (var task in userTasksOnBoard)
        {
            task.AssignedUserId = null;
            dbContext.Update(task);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<ICollection<Taskboard>> GetBoardsByUserId(Guid userId) =>
        await dbContext.Taskboards
            .Include(t => t.TaskboardUsers!)
            .ThenInclude(t => t.User)
            .Where(b => b.TaskboardUsers!.Any(uta => uta.UserId == userId))
            .ToListAsync();

    public async Task<ICollection<Taskboard>> GetTenantBoards(Guid tenantId) =>
        await dbContext.Taskboards
            .Include(t => t.TaskboardUsers!)
            .ThenInclude(t => t.User)
            .Where(b => b.TenantId == tenantId)
            .ToListAsync();

    public async Task DeleteBoard(Taskboard board)
    {
        dbContext.Remove(board);
        await dbContext.SaveChangesAsync();
    }
}
