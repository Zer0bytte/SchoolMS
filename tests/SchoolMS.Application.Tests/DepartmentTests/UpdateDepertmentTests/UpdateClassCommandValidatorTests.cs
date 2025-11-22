using SchoolMS.Application.Features.Classes.Commands.UpdateClass;

namespace SchoolMS.Application.Tests.DepartmentTests.UpdateDepertmentTests;

public class UpdateClassCommandValidatorTests
{

    [Fact]
    public void Validate_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateClassCommand
        {
            Name = new string('A', 101),
            Semester = "Fall",
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 1, 2)
        };

        var validator = new UpdateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSemesterExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateClassCommand
        {
            Name = "Valid Name",
            Semester = new string('B', 51),
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 1, 2)
        };

        var validator = new UpdateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Semester");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var command = new UpdateClassCommand
        {
            Name = "Valid Name",
            Semester = "Fall",
            StartDate = new DateOnly(2025, 2, 1),
            EndDate = new DateOnly(2025, 1, 1)
        };

        var validator = new UpdateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.ErrorMessage == "End date must be on or after the start date.");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenOnlyNameIsValid()
    {
        // Arrange
        var command = new UpdateClassCommand
        {
            Name = "Valid Name"
        };

        var validator = new UpdateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => true);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenOnlySemesterIsValid()
    {
        // Arrange
        var command = new UpdateClassCommand
        {
            Semester = "Fall"
        };

        var validator = new UpdateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => true);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDatesAreValid()
    {
        // Arrange
        var command = new UpdateClassCommand
        {
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 1, 10)
        };

        var validator = new UpdateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => true);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDatesAreMissing()
    {
        // Arrange
        var command = new UpdateClassCommand
        {
            Name = "Valid Name",
            Semester = "Fall"
        };

        var validator = new UpdateClassCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => true);
    }
}

