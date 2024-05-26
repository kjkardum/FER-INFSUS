using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Create;
using FluentAssertions;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Commands;

public class TaskItemCreateCommandValidatorTests
{
    private readonly TaskItemCreateCommandValidator _validator;

    public TaskItemCreateCommandValidatorTests()
    {
        _validator = new TaskItemCreateCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsTrue()
    {
        // Arrange
        var command = new TaskItemCreateCommand
        {
            Name = "TaskItem Name",
            Description = string.Empty,
            TaskboardId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ValidLongCommand_ReturnsTrue()
    {
        // Arrange
        var command = new TaskItemCreateCommand
        {
            Name = new string('a', 100),
            Description = string.Empty,
            TaskboardId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_EmptyName_ReturnsFalse(string name)
    {
        // Arrange
        var command = new TaskItemCreateCommand
        {
            Name = name,
            Description = string.Empty,
            TaskboardId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TooLongName_ReturnsFalse()
    {
        // Arrange
        var command = new TaskItemCreateCommand
        {
            Name = new string('a', 101),
            Description = string.Empty,
            TaskboardId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EmptyGuid_ReturnsFalse()
    {
        // Arrange
        var command = new TaskItemCreateCommand
        {
            Name = "TaskItem Name",
            Description = string.Empty,
            TaskboardId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
