using SchoolMS.Application.Features.Courses.Commands.CreateCourse;

namespace SchoolMS.Application.Tests.Courses.CreateCourse;


public class CreateCourseCommandValidatorTests
{

    [Fact]
    public static void Validate_GivenNameIsEmpty_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Name = string.Empty };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage == "Course name is required.");
    }

    [Fact]
    public static void Validate_GivenNameIsNull_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Name = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public static void Validate_GivenNameExceedsLimit_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Name = new string('A', 201) };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage == "Course name must not exceed 200 characters.");
    }

    [Fact]
    public static void Validate_GivenNameIsValid_ShouldNotReturnValidationErrorsForName()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Computer Science",
            Code = "CS101",
            Description = "Basic computer science course",
            DepartmentId = Guid.NewGuid(),
            Credits = 3
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public static void Validate_GivenNameIsExactly200Characters_ShouldNotReturnValidationErrorsForName()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = new string('A', 200),
            Code = "CS101",
            Description = "Valid description",
            DepartmentId = Guid.NewGuid(),
            Credits = 3
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Name");
    }




    [Fact]
    public static void Validate_GivenCodeIsEmpty_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Code = string.Empty };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Code" && e.ErrorMessage == "Course code is required.");
    }

    [Fact]
    public static void Validate_GivenCodeIsNull_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Code = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Code");
    }

    [Fact]
    public static void Validate_GivenCodeExceedsLimit_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Code = new string('A', 21) };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Code" && e.ErrorMessage == "Course code must not exceed 20 characters.");
    }

    [Fact]
    public static void Validate_GivenCodeIsValid_ShouldNotReturnValidationErrorsForCode()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Computer Science",
            Code = "CS101",
            Description = "Basic computer science course",
            DepartmentId = Guid.NewGuid(),
            Credits = 3
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Code");
    }




    [Fact]
    public static void Validate_GivenDescriptionIsEmpty_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Description = string.Empty };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description" && e.ErrorMessage == "Description is required.");
    }

    [Fact]
    public static void Validate_GivenDescriptionIsNull_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Description = null! };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public static void Validate_GivenDescriptionExceedsLimit_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Description = new string('A', 1001) };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description" && e.ErrorMessage == "Description must not exceed 1000 characters.");
    }

    [Fact]
    public static void Validate_GivenDescriptionIsValid_ShouldNotReturnValidationErrorsForDescription()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Computer Science",
            Code = "CS101",
            Description = "This is a comprehensive introduction to computer science.",
            DepartmentId = Guid.NewGuid(),
            Credits = 3
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Description");
    }




    [Fact]
    public static void Validate_GivenDepartmentIdIsEmpty_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { DepartmentId = Guid.Empty };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DepartmentId" && e.ErrorMessage == "Department is required.");
    }

    [Fact]
    public static void Validate_GivenDepartmentIdIsValid_ShouldNotReturnValidationErrorsForDepartmentId()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Computer Science",
            Code = "CS101",
            Description = "Basic computer science course",
            DepartmentId = Guid.NewGuid(),
            Credits = 3
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "DepartmentId");
    }




    [Fact]
    public static void Validate_GivenCreditsIsZero_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Credits = 0 };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Credits" && e.ErrorMessage == "Credits must be greater than zero.");
    }

    [Fact]
    public static void Validate_GivenCreditsIsNegative_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand { Credits = -1 };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Credits");
    }

    [Fact]
    public static void Validate_GivenCreditsIsValid_ShouldNotReturnValidationErrorsForCredits()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Computer Science",
            Code = "CS101",
            Description = "Basic computer science course",
            DepartmentId = Guid.NewGuid(),
            Credits = 3
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Credits");
    }

    [Fact]
    public static void Validate_GivenCreditsIsOne_ShouldNotReturnValidationErrorsForCredits()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Computer Science",
            Code = "CS101",
            Description = "Basic computer science course",
            DepartmentId = Guid.NewGuid(),
            Credits = 1
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Credits");
    }




    [Fact]
    public static void Validate_GivenAllPropertiesAreValid_ShouldNotReturnValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Computer Science",
            Code = "CS101",
            Description = "A comprehensive introduction to computer science fundamentals.",
            DepartmentId = Guid.NewGuid(),
            Credits = 3
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public static void Validate_GivenAllPropertiesAreInvalid_ShouldReturnMultipleValidationErrors()
    {
        // Arrange
        var validator = new CreateCourseCommandValidator();
        var command = new CreateCourseCommand
        {
            Name = string.Empty,
            Code = string.Empty,
            Description = string.Empty,
            DepartmentId = Guid.Empty,
            Credits = 0
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        Assert.Contains(result.Errors, e => e.PropertyName == "Code");
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
        Assert.Contains(result.Errors, e => e.PropertyName == "DepartmentId");
        Assert.Contains(result.Errors, e => e.PropertyName == "Credits");
    }


}