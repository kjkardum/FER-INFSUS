using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Application.Repositories;

public interface ITaskboardRepository
{
    Task CreateBoard(Taskboard board);
    Task UpdateBoard(Taskboard board);
    Task<Taskboard?> GetBoardById(Guid boardId);
    Task AddUserToBoard(Guid boardId, Guid userId);
    Task RemoveUserFromBoard(Guid boardId, Guid userId);
    Task<ICollection<Taskboard>> GetBoardsByUserId(Guid userId);
    Task<ICollection<Taskboard>> GetTenantBoards(Guid tenantId);
    Task DeleteBoard(Taskboard board);
}
