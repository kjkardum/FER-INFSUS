using Bogus;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Domain.Entities;
using FER.InfSus.Time.Domain.Enums;
using FER.InfSus.Time.Infrastructure.Persistence;
using FER.InfSus.Time.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FER.InfSus.Time.Infrastructure.Tests.Repositories;

public class TaskItemRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITaskItemRepository _repository;
    private readonly SqliteConnection _connection;
    private readonly Tenant _tenant;
    private readonly Taskboard _taskboard;

    public TaskItemRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_connection).Options;
        _dbContext = new ApplicationDbContext(options);
        _dbContext.Database.EnsureCreated();

        _repository = new TaskItemRepository(_dbContext);

        _tenant = new Faker<Tenant>()
            .RuleFor(o => o.Id, _ => Guid.NewGuid())
            .RuleFor(o => o.Name, f => f.Company.CompanyName())
            .RuleFor(o => o.Address, f => f.Address.FullAddress())
            .RuleFor(o => o.Users, _ => new List<User>())
            .RuleFor(o => o.Taskboards, _ => new List<Taskboard>())
            .RuleFor(o => o.Reports, _ => new List<Report>())
            .Generate();

        _taskboard = new Faker<Taskboard>()
            .RuleFor(tb => tb.Id, _ => Guid.NewGuid())
            .RuleFor(tb => tb.Name, f => f.Commerce.ProductName())
            .RuleFor(tb => tb.Description, f => f.Lorem.Sentence())
            .RuleFor(tb => tb.TenantId, _ => _tenant.Id)
            .Generate();

        _dbContext.Tenants.Add(_tenant);
        _dbContext.Taskboards.Add(_taskboard);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task Create_TaskItem_Returns_TaskItem()
    {
        // Arrange
        var taskItem = new Faker<TaskItem>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.Name, f => f.Commerce.ProductName())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.State, _ => TaskItemState.Novo)
            .RuleFor(t => t.TaskboardId, _ => _taskboard.Id)
            .RuleFor(t => t.CreatedAt, _ => DateTime.UtcNow)
            .Generate();

        // Act
        await _repository.Create(taskItem);

        // Assert
        var result = await _repository.GetById(taskItem.Id);
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskItem.Id);
        result.Name.Should().Be(taskItem.Name);
        result.Description.Should().Be(taskItem.Description);
        result.State.Should().Be(taskItem.State);
        result.TaskboardId.Should().Be(taskItem.TaskboardId);
        result.CreatedAt.Should().BeCloseTo(taskItem.CreatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Update_TaskItem_Updates_TaskItem()
    {
        // Arrange
        var taskItem = new Faker<TaskItem>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.Name, f => f.Commerce.ProductName())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.State, _ => TaskItemState.Novo)
            .RuleFor(t => t.TaskboardId, _ => _taskboard.Id)
            .RuleFor(t => t.CreatedAt, _ => DateTime.UtcNow)
            .Generate();

        await _repository.Create(taskItem);

        // Act
        taskItem.Name = "UpdatedTaskItemName";
        await _repository.Update(taskItem);

        // Assert
        var result = await _repository.GetById(taskItem.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("UpdatedTaskItemName");
    }

    [Fact]
    public async Task Delete_TaskItem_Deletes_TaskItem()
    {
        // Arrange
        var taskItem = new Faker<TaskItem>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.Name, f => f.Commerce.ProductName())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.State, _ => TaskItemState.Novo)
            .RuleFor(t => t.TaskboardId, _ => _taskboard.Id)
            .RuleFor(t => t.CreatedAt, _ => DateTime.UtcNow)
            .Generate();

        await _repository.Create(taskItem);

        // Act
        await _repository.Delete(taskItem);

        // Assert
        var result = await _repository.GetById(taskItem.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetById_Returns_TaskItem()
    {
        // Arrange
        var taskItem = new Faker<TaskItem>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.Name, f => f.Commerce.ProductName())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.State, _ => TaskItemState.Novo)
            .RuleFor(t => t.TaskboardId, _ => _taskboard.Id)
            .RuleFor(t => t.CreatedAt, _ => DateTime.UtcNow)
            .Generate();

        await _repository.Create(taskItem);

        // Act
        var result = await _repository.GetById(taskItem.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskItem.Id);
    }

    [Fact]
    public async Task AddHistoryLog_Adds_HistoryLog()
    {
        // Arrange
        var taskItem = new Faker<TaskItem>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.Name, f => f.Commerce.ProductName())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.State, _ => TaskItemState.Novo)
            .RuleFor(t => t.TaskboardId, _ => _taskboard.Id)
            .RuleFor(t => t.CreatedAt, _ => DateTime.UtcNow)
            .Generate();

        await _repository.Create(taskItem);

        var historyLog = new Faker<TaskItemHistoryLog>()
            .RuleFor(h => h.Id, _ => Guid.NewGuid())
            .RuleFor(h => h.TaskItemId, _ => taskItem.Id)
            .RuleFor(h => h.Changelog, f => f.Lorem.Sentence())
            .RuleFor(h => h.ModifiedAt, _ => DateTime.UtcNow)
            .Generate();

        // Act
        await _repository.AddHistoryLog(historyLog);

        // Assert
        var result = await _repository.GetById(taskItem.Id);
        result.Should().NotBeNull();
        result!.HistoryLogs.Should().ContainSingle();
        result.HistoryLogs.First().Id.Should().Be(historyLog.Id);
    }

    [Fact]
    public async Task GetByAssignedUserId_Returns_TaskItems()
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

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        var taskItems = new Faker<TaskItem>()
            .RuleFor(t => t.Id, _ => Guid.NewGuid())
            .RuleFor(t => t.Name, f => f.Commerce.ProductName())
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.State, _ => TaskItemState.Novo)
            .RuleFor(t => t.TaskboardId, _ => _taskboard.Id)
            .RuleFor(t => t.AssignedUserId, _ => user.Id)
            .RuleFor(t => t.CreatedAt, _ => DateTime.UtcNow)
            .Generate(5);

        await _dbContext.TaskItems.AddRangeAsync(taskItems);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByAssignedUserId(user.Id);

        // Assert
        result.Should().HaveCount(5);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Close();
        GC.SuppressFinalize(this);
    }
}
