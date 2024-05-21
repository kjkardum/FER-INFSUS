using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Infrastructure.Persistence;
using FER.InfSus.Time.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FER.InfSus.Time.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserRepository _repository;
    private readonly SqliteConnection _connection;
    private readonly Tenant _tenant;

    public UserRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_connection).Options;
        _dbContext = new ApplicationDbContext(options);
        _dbContext.Database.EnsureCreated();

        _repository = new UserRepository(_dbContext);

        _tenant = new Faker<Tenant>()
            .RuleFor(o => o.Id, _ => Guid.Empty)
            .RuleFor(o => o.Name, f => f.Company.CompanyName())
            .RuleFor(o => o.Address, f => f.Address.FullAddress())
            .RuleFor(o => o.Users, _ => new List<User>())
            .RuleFor(o => o.Taskboards, _ => new List<Taskboard>())
            .RuleFor(o=> o.Reports, _ => new List<Report>())
            .Generate();
        _dbContext.Tenants.Add(_tenant);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task Create_User_Returns_User()
    {
        // Arrange
        var cancellationToken = default(CancellationToken);
        var user = new Faker<User>()
            .RuleFor(t => t.Id, _ => Guid.Empty)
            .RuleFor(t => t.FirstName, f => f.Person.FirstName)
            .RuleFor(t => t.LastName, f => f.Person.LastName)
            .RuleFor(t => t.Email, f => f.Internet.Email())
            .RuleFor(t => t.PasswordHash, f => f.Internet.Password())
            .RuleFor(t => t.TenantId, _ => _tenant.Id)
            .Generate();

        // Act
        var createdUser = await _repository.Create(user);

        var result = await _repository.GetByUserId(createdUser.Id);
        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.TenantId.Should().Be(user.TenantId);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.Email.Should().Be(user.Email);
        result.PasswordHash.Should().Be(user.PasswordHash);
        result.TenantId.Should().Be(user.TenantId);
    }

    #region Dispose

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) => _dbContext.Dispose();

    #endregion
}
