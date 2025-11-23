using SchoolMS.Application.Features.Classes.Commands.CreateClass;

namespace SchoolMS.Application.Tests.ClassTests.CreateClassTests;

public class CreateClassCommandValidatorTests
{
    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = "",
            CourseId = Guid.NewGuid(),
            Semester = "Fall",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 2, 1)
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = new string('A', 101),
            CourseId = Guid.NewGuid(),
            Semester = "Fall",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 2, 1)
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCourseIdIsEmpty()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = "Valid Name",
            CourseId = Guid.Empty,
            Semester = "Fall",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 2, 1)
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "CourseId");

    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSemesterIsEmpty()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = "Valid Name",
            CourseId = Guid.NewGuid(),
            Semester = "",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 2, 1)
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Semester");

    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSemesterExceedsMaxLength()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = "Valid Name",
            CourseId = Guid.NewGuid(),
            Semester = new string('B', 51),
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 2, 1)
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Semester");

    }

    [Fact]
    public void Validate_ShouldHaveError_WhenStartDateIsDefault()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = "Valid Name",
            CourseId = Guid.NewGuid(),
            Semester = "Fall",
            StartDate = default,
            EndDate = new DateOnly(2025, 2, 1)
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "StartDate");

    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEndDateIsDefault()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = "Valid Name",
            CourseId = Guid.NewGuid(),
            Semester = "Fall",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = default
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = "Valid Name",
            CourseId = Guid.NewGuid(),
            Semester = "Fall",
            StartDate = new DateOnly(2025, 2, 1),
            EndDate = new DateOnly(2025, 1, 1)
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateClassCommand
        {
            Name = "Valid Name",
            CourseId = Guid.NewGuid(),
            Semester = "Fall",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 2, 1)
        };

        var validator = new CreateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.Errors.Any());
    }
}