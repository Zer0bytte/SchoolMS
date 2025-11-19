using SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Tests.Departments.UpdateDepertmentTests;

public class UpdateDeparmentCommandValidatorTests
{
 

    [Fact]
    public void Validate_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateDepartmentCommand
        {
            Id = Guid.NewGuid(),
            Name = new string('A', 101)
        };
        var validator = new UpdateDepartmentCommandValidator();
        // Act
        var result = validator.Validate(command);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateDepartmentCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Name",
            Description = new string('B', 501)
        };
        var validator = new UpdateDepartmentCommandValidator();
        // Act
        var result = validator.Validate(command);
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_ShouldBeValid_WhenAllFieldsAreValid()
    {
        // Arrange
        var command = new UpdateDepartmentCommand
        {
            Id = Guid.NewGuid(),
            Name = "Valid Department Name",
            Description = "Valid Description"
        };
        var validator = new UpdateDepartmentCommandValidator();
        // Act
        var result = validator.Validate(command);
        // Assert
        Assert.True(result.IsValid);
    }
}
