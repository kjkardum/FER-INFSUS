using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Infrastructure.Persistence;

namespace FER.InfSus.Time.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> Create(User user)
    {
        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateLastLogin(User user)
    {
        user.LastLogin = DateTime.UtcNow;
        _dbContext.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public Task<ICollection<User>> GetPaginated(Guid tenantId, int page, int pageSize, string? orderBy, string? filterBy)
        => GetPaginated(GetAsQueryable(), tenantId, page, pageSize, orderBy, filterBy);

    public async Task<ICollection<User>> GetPaginated(
        IQueryable<User> query,
        Guid tenantId,
        int page,
        int pageSize,
        string? orderBy,
        string? filterBy)
    {
        var filterString = filterBy ?? string.Empty;

        query = query.Where(t => t.TenantId == tenantId)
            .Where(t => t.Email.Contains(filterString) ||
                t.FirstName.Contains(filterString) ||
                t.LastName.Contains(filterString)
        );

        orderBy += ",-,-"; // So I can split safely without checks.
        Expression<Func<User, object?>> sortFunc = orderBy.Split(',')[0] switch
        {
            "email" => t => t.Email,
            "firstName" => t => t.FirstName,
            "lastName" => t => t.LastName,
            _ => t => t.Email,
        };

        query = orderBy.Split(',')[1] switch
        {
            "desc" => query.OrderByDescending(sortFunc),
            _ => query.OrderBy(sortFunc),
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public IQueryable<User> GetAsQueryable() => _dbContext.Users.AsQueryable();

    public Task<int> CountUsers() => _dbContext.Users.CountAsync();
    public Task Update(User user)
    {
        _dbContext.Update(user);
        return _dbContext.SaveChangesAsync();
    }

    public Task Delete(User user)
    {
        _dbContext.Remove(user);
        return _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetByEmail(string email)
        => await _dbContext.Users.SingleOrDefaultAsync(t => t.Email == email);

    public Task<User?> GetByUserId(Guid userId)
        => _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId);

    public Task<bool> DoesUserExist(string email)
        => _dbContext.Users.AnyAsync(t => t.Email == email);
}
