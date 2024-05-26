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

public class TaskboardRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITaskboardRepository _repository;
    private readonly SqliteConnection _connection;
    private readonly Tenant _tenant;

    public TaskboardRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_connection).Options;
        _dbContext = new ApplicationDbContext(options);
        _dbContext.Database.EnsureCreated();

        _repository = new TaskboardRepository(_dbContext);

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
    public async Task CreateBoard_Taskboard_Returns_Taskboard()
    {
        // Arrange
        var taskboard = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate();

        // Act
        await _repository.CreateBoard(taskboard);

        // Assert
        var result = await _repository.GetBoardById(taskboard.Id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskboard.Id);
        result.Name.Should().Be(taskboard.Name);
        result.TenantId.Should().Be(taskboard.TenantId);
    }

    [Fact]
    public async Task UpdateBoard_Taskboard_Updates_Taskboard()
    {
        // Arrange
        var taskboard = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate();

        await _repository.CreateBoard(taskboard);

        // Act
        taskboard.Name = "UpdatedTaskboardName";
        await _repository.UpdateBoard(taskboard);

        // Assert
        var result = await _repository.GetBoardById(taskboard.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("UpdatedTaskboardName");
    }

    [Fact]
    public async Task DeleteBoard_Taskboard_Deletes_Taskboard()
    {
        // Arrange
        var taskboard = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate();

        await _repository.CreateBoard(taskboard);

        // Act
        await _repository.DeleteBoard(taskboard);

        // Assert
        var result = await _repository.GetBoardById(taskboard.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBoardById_Returns_Taskboard()
    {
        // Arrange
        var taskboard = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate();

        await _repository.CreateBoard(taskboard);

        // Act
        var result = await _repository.GetBoardById(taskboard.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskboard.Id);
    }

    [Fact]
    public async Task AddUserToBoard_Adds_UserToBoard()
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

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var taskboard = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate();

        await _repository.CreateBoard(taskboard);

        // Act
        await _repository.AddUserToBoard(taskboard.Id, user.Id);

        // Assert
        var result = await _repository.GetBoardById(taskboard.Id);
        result.Should().NotBeNull();
        result!.TaskboardUsers.Should().ContainSingle();
        result.TaskboardUsers.First().UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task RemoveUserFromBoard_Removes_UserFromBoard()
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

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var taskboard = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate();

        await _repository.CreateBoard(taskboard);
        await _repository.AddUserToBoard(taskboard.Id, user.Id);

        // Act
        await _repository.RemoveUserFromBoard(taskboard.Id, user.Id);

        // Assert
        var result = await _repository.GetBoardById(taskboard.Id);
        result.Should().NotBeNull();
        result!.TaskboardUsers.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBoardsByUserId_Returns_Taskboards()
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

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var taskboards = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate(3);

        await _dbContext.Taskboards.AddRangeAsync(taskboards);
        await _dbContext.SaveChangesAsync();

        foreach (var taskboard in taskboards)
        {
            await _repository.AddUserToBoard(taskboard.Id, user.Id);
        }

        // Act
        var result = await _repository.GetBoardsByUserId(user.Id);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetTenantBoards_Returns_Taskboards()
    {
        // Arrange
        var taskboards = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate(3);

        await _dbContext.Taskboards.AddRangeAsync(taskboards);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetTenantBoards(_tenant.Id);

        // Assert
        result.Should().HaveCount(3);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Close();
        GC.SuppressFinalize(this);
    }
}
