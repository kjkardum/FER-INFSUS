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

public class UserRepositoryTests : IDisposable
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
            .RuleFor(o => o.Id, _ => Guid.NewGuid())
            .RuleFor(o => o.Name, f => f.Company.CompanyName())
            .RuleFor(o => o.Address, f => f.Address.FullAddress())
            .RuleFor(o => o.Users, _ => new List<User>())
            .RuleFor(o => o.Taskboards, _ => new List<Taskboard>())
            .RuleFor(o => o.Reports, _ => new List<Report>())
            .Generate();
        _dbContext.Tenants.Add(_tenant);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task Create_User_Returns_User()
    {
        // Arrange
        var user = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.TenantId, _ => _tenant.Id)
            .Generate();

        // Act
        var createdUser = await _repository.Create(user);

        // Assert
        var result = await _repository.GetByUserId(createdUser.Id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.TenantId.Should().Be(user.TenantId);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.Email.Should().Be(user.Email);
        result.PasswordHash.Should().Be(user.PasswordHash);
    }

    [Fact]
    public async Task UpdateLastLogin_Updates_LastLogin()
    {
        // Arrange
        var user = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.TenantId, _ => _tenant.Id)
            .Generate();

        var createdUser = await _repository.Create(user);

        // Act
        var updatedUser = await _repository.UpdateLastLogin(createdUser);

        // Assert
        var result = await _repository.GetByUserId(updatedUser.Id);
        result.Should().NotBeNull();
        result!.LastLogin.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetPaginated_Returns_Paginated_Users()
    {
        // Arrange
        var users = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.TenantId, _ => _tenant.Id)
            .Generate(10);

        _dbContext.Users.AddRange(users);
        _dbContext.SaveChanges();

        // Act
        var result = await _repository.GetPaginated(_tenant.Id, 1, 5, "email", null);

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task Update_User_Updates_User()
    {
        // Arrange
        var user = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.TenantId, _ => _tenant.Id)
            .Generate();

        var createdUser = await _repository.Create(user);

        // Act
        createdUser.FirstName = "UpdatedFirstName";
        await _repository.Update(createdUser);

        // Assert
        var result = await _repository.GetByUserId(createdUser.Id);
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("UpdatedFirstName");
    }

    [Fact]
    public async Task Delete_User_Deletes_User()
    {
        // Arrange
        var user = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.TenantId, _ => _tenant.Id)
            .Generate();

        var createdUser = await _repository.Create(user);

        // Act
        await _repository.Delete(createdUser);

        // Assert
        var result = await _repository.GetByUserId(createdUser.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmail_Returns_User()
    {
        // Arrange
        var email = new Faker().Internet.Email();
        var user = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, _ => email)
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.TenantId, _ => _tenant.Id)
            .Generate();

        await _repository.Create(user);

        // Act
        var result = await _repository.GetByEmail(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task DoesUserExist_Returns_True_If_User_Exists()
    {
        // Arrange
        var email = new Faker().Internet.Email();
        var user = new Faker<User>()
            .RuleFor(u => u.Id, _ => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, _ => email)
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.TenantId, _ => _tenant.Id)
            .Generate();

        await _repository.Create(user);

        // Act
        var result = await _repository.DoesUserExist(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DoesUserExist_Returns_False_If_User_Does_Not_Exist()
    {
        // Arrange
        var email = new Faker().Internet.Email();

        // Act
        var result = await _repository.DoesUserExist(email);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Close();
        GC.SuppressFinalize(this);
    }
}
