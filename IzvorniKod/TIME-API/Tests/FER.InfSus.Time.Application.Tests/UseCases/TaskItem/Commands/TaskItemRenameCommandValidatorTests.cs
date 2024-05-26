using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Rename;
using FluentAssertions;
using Xunit;

namespace FER.InfSus.Time.Application.Tests.UseCases.TaskItem.Commands;

public class TaskItemRenameCommandValidatorTests
{
    private readonly TaskItemRenameCommandValidator _validator;

    public TaskItemRenameCommandValidatorTests()
    {
        _validator = new TaskItemRenameCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsTrue()
    {
        // Arrange
        var command = new TaskItemRenameCommand
        {
            NewName = "TaskItem Name",
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
        var command = new TaskItemRenameCommand
        {
            NewName = new string('a', 100),
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
        var command = new TaskItemRenameCommand
        {
            NewName = name,
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
        var command = new TaskItemRenameCommand
        {
            NewName = new string('a', 101),
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

}
