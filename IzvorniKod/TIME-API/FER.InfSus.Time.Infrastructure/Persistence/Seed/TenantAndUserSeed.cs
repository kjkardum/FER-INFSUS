using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FER.InfSus.Time.Infrastructure.Persistence.Seed;

public static class TenantAndUserSeed
{
    public static async Task Seed(ApplicationDbContext context)
    {
        if (!await context.Tenants.AnyAsync())
        {
            var tenant = new Tenant
            {
                Id = Guid.Empty,
                Name = "Tenant #1",
                Address = "Unska 3, Zagreb, HR",
                Users = new List<User>
                {
                    new()
                    {
                        Id = Guid.Empty,
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "USER@TIME.COM",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pa$$w0rd"),
                        UserType = UserType.ADMIN,
                    }
                },
                Reports = null,
                Taskboards = new List<Taskboard>
                {
                    new()
                    {
                        Id = Guid.Empty,
                        TenantId = Guid.Empty,
                        Tenant = null,
                        Name = "Taskboard 1",
                        Description = "This is a seeded taskboard.",
                        TaskItems = null,
                        TaskboardUsers = null
                    }
                }
            };

            await context.Tenants.AddAsync(tenant);
            await context.SaveChangesAsync();
        }
    }
}
