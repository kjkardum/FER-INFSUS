using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Create;
using FluentAssertions;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.Taskboard.Commands;

public class TaskboardCreateCommandValidatorTests
{
    private readonly TaskboardCreateCommandValidator _validator;

    public TaskboardCreateCommandValidatorTests()
    {
        _validator = new TaskboardCreateCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsTrue()
    {
        // Arrange
        var command = new TaskboardCreateCommand
        {
            Name = "Taskboard Name",
            Description = "Taskboard Description"
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
        var command = new TaskboardCreateCommand
        {
            Name = new string('a', 100),
            Description = "Taskboard Description"
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
        var command = new TaskboardCreateCommand
        {
            Name = name,
            Description = "Taskboard Description"
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
        var command = new TaskboardCreateCommand
        {
            Name = new string('a', 101),
            Description = "Taskboard Description"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TooLongDescription_ReturnsFalse()
    {
        // Arrange
        var command = new TaskboardCreateCommand
        {
            Name = "Taskboard Name",
            Description = new string('a', 1001)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_LongDescription_ReturnsTrue()
    {
        // Arrange
        var command = new TaskboardCreateCommand
        {
            Name = "Taskboard Name",
            Description = new string('a', 1000)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
