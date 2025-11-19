using SchoolMS.Application.Features.Departments.Commands.CreateDepartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Tests.Departments.CreateDepartmentTests;

public class CreateDepartmentCommandValidatorTests
{
    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "",
            Description = "Valid Description",
            HeadOfDepartmentId = Guid.NewGuid()
        };

        var validator = new CreateDepartmentCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.Errors.Any(e => e.PropertyName == "Name"));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionIsEmpty()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "Valid Name",
            Description = "",
            HeadOfDepartmentId = Guid.NewGuid()
        };
        var validator = new CreateDepartmentCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.Errors.Any(e => e.PropertyName == "Description"));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "Valid Name",
            Description = "Valid Description",
            HeadOfDepartmentId = Guid.NewGuid()
        };
        var validator = new CreateDepartmentCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.Errors.Any());
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = new string('A', 101), // 101 characters
            Description = "Valid Description",
            HeadOfDepartmentId = Guid.NewGuid()
        };
        var validator = new CreateDepartmentCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.Errors.Any(e => e.PropertyName == "Name"));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            Name = "Valid Name",
            Description = new string('B', 501),
            HeadOfDepartmentId = Guid.NewGuid()
        };
        var validator = new CreateDepartmentCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.Errors.Any(e => e.PropertyName == "Description"));
    }
}
