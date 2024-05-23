using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Application.Repositories;

public interface IUserRepository
{
    Task<User> Create(User user);
    Task<User?> GetByEmail(string email);
    Task<User?> GetByUserId(Guid userId);
    Task<bool> DoesUserExist(string email);
    Task<User> UpdateLastLogin(User user);
    Task<ICollection<User>> GetPaginated(Guid tenantId, int page, int pageSize, string? orderBy, string? filterBy);

    Task<ICollection<User>> GetPaginated(
        IQueryable<User> query,
        Guid tenantId,
        int page,
        int pageSize,
        string? orderBy,
        string? filterBy);

    IQueryable<User> GetAsQueryable();
    Task<int> CountUsers();
}
