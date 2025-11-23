using SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;

namespace SchoolMS.Application.Tests.AssignmentTests.CreateAssignment;
public class CreateAssignmentCommandValidatorTests
{
    private readonly CreateAssignmentCommandValidator _validator;

    public CreateAssignmentCommandValidatorTests()
    {
        _validator = new CreateAssignmentCommandValidator();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenClassIdIsEmpty()
    {
        // Arrange
        var command = new CreateAssignmentCommand
        {
            ClassId = Guid.Empty,
            Title = "Test",
            Description = "Test",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "ClassId");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty()
    {
        // Arrange
        var command = new CreateAssignmentCommand
        {
            ClassId = Guid.NewGuid(),
            Title = "",
            Description = "Test",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var command = new CreateAssignmentCommand
        {
            ClassId = Guid.NewGuid(),
            Title = new string('A', 201),
            Description = "Test",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsEmpty()
    {
        // Arrange
        var command = new CreateAssignmentCommand
        {
            ClassId = Guid.NewGuid(),
            Title = "Title",
            Description = "",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var command = new CreateAssignmentCommand
        {
            ClassId = Guid.NewGuid(),
            Title = "Title",
            Description = new string('B', 2001),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDueDateIsNotInFuture()
    {
        // Arrange
        var command = new CreateAssignmentCommand
        {
            ClassId = Guid.NewGuid(),
            Title = "Test",
            Description = "Test",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "DueDate");
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateAssignmentCommand
        {
            ClassId = Guid.NewGuid(),
            Title = "Valid Title",
            Description = "Valid Description",
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2))
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.Empty(result.Errors);
    }
}
