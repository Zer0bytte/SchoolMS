using SchoolMS.Application.Features.Courses.Commands.UpdateCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Tests.Courses.UpdateCourse;

public class UpdateCourseCommandValidatorTests
{
    [Fact]
    public static void Validate_GivenCourseNameExceedsLimit_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new UpdateCourseCommandValidator();
        var command = new UpdateCourseCommand
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 201),
            Description = "",
            Credits = 1,
            Code = "",
            DepartmentId = Guid.Empty
        };
        // Act
        var result = validator.Validate(command);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public static void Validate_GivenCourseCodeExceedsLimit_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new UpdateCourseCommandValidator();
        var command = new UpdateCourseCommand
        {
            Id = Guid.NewGuid(),
            Name = "",
            Description = "",
            Credits = 1,
            Code = new string('A', 21),
            DepartmentId = Guid.Empty
        };
        // Act
        var result = validator.Validate(command);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Code");
    }

    [Fact]
    public static void Validate_GivenCourseDescriptionExceedsLimit_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new UpdateCourseCommandValidator();
        var command = new UpdateCourseCommand
        {
            Id = Guid.NewGuid(),
            Name = "",
            Description = new string('A', 1001),
            Credits = 1,
            Code = "",
            DepartmentId = Guid.Empty
        };
        // Act
        var result = validator.Validate(command);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public static void Validate_GivenCourseCreditsUnderZero_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new UpdateCourseCommandValidator();
        var command = new UpdateCourseCommand
        {
            Id = Guid.NewGuid(),
            Name = "",
            Description = new string('A', 1001),
            Credits = -1,
            Code = "",
            DepartmentId = Guid.Empty
        };
        // Act
        var result = validator.Validate(command);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Credits");
    }

    [Fact]
    public static void Validate_GivenCourseCreditsEqualZero_ShouldReturnValidationErrors()
    {
        // Arrange
        var validator = new UpdateCourseCommandValidator();
        var command = new UpdateCourseCommand
        {
            Id = Guid.NewGuid(),
            Name = "",
            Description = new string('A', 1001),
            Credits = 0,
            Code = "",
            DepartmentId = Guid.Empty
        };
        // Act
        var result = validator.Validate(command);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Credits");
    }
}
