using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Tests.Departments.UpdateDepertmentTests;

public class UpdateDepartmentCommandHandlerTests
{
    private TestAppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestAppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new TestAppDbContext(options);
    }

    [Fact]
    public async Task Handle_GivenValidRequest_ShouldUpdateDepartment()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var department = Department.Create(Guid.NewGuid(), "Mathematics", "Department of Mathematics", Guid.NewGuid()).Value;

        context.Departments.Add(department);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context);
        var command = new UpdateDepartmentCommand
        {
            Id = department.Id,
            Name = "Advanced Mathematics",
            Description = "Department of Advanced Mathematics"
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Advanced Mathematics", result.Value.Name);
        Assert.Equal("Department of Advanced Mathematics", result.Value.Description);
        var updatedDepartmentInDb = await context.Departments.FindAsync(department.Id);
        Assert.NotNull(updatedDepartmentInDb);
        Assert.Equal("Advanced Mathematics", updatedDepartmentInDb!.Name);
        Assert.Equal("Department of Advanced Mathematics", updatedDepartmentInDb.Description);
    }

    [Fact]
    public async Task Handle_GivenInvalidDepartmentId_ShouldReturnError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var handler = new UpdateDepartmentCommandHandler(context);
        var command = new UpdateDepartmentCommand
        {
            Id = Guid.NewGuid(), // Non-existent department ID
            Name = "Physics",
            Description = "Department of Physics"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DepartmentErrors.NotFound, result.Errors.First());
    }

    [Fact]
    public async Task Handle_GivenDuplicateDepartmentName_ShouldReturnError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var department1 = Department.Create(Guid.NewGuid(), "Biology", "Department of Biology", Guid.NewGuid()).Value;
        var department2 = Department.Create(Guid.NewGuid(), "Chemistry", "Department of Chemistry", Guid.NewGuid()).Value;
        context.Departments.Add(department1);
        context.Departments.Add(department2);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context);
        var command = new UpdateDepartmentCommand
        {
            Id = department2.Id,
            Name = "Biology", // Duplicate name
            Description = "Updated Description"
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DepartmentErrors.DublicateName, result.Errors.First());
    }

    [Fact]
    public async Task Handle_GivenSameName_ShouldUpdateDepartmentSuccessfully()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var department = Department.Create(Guid.NewGuid(), "History", "Department of History", Guid.NewGuid()).Value;
        context.Departments.Add(department);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context);
        var command = new UpdateDepartmentCommand
        {
            Id = department.Id,
            Name = "History", // Same name
            Description = "Updated Department of History"
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("History", result.Value.Name);
        Assert.Equal("Updated Department of History", result.Value.Description);
        var updatedDepartmentInDb = await context.Departments.FindAsync(department.Id);
        Assert.NotNull(updatedDepartmentInDb);
        Assert.Equal("History", updatedDepartmentInDb!.Name);
        Assert.Equal("Updated Department of History", updatedDepartmentInDb.Description);
    }

    [Fact]
    public async Task Handle_GivenInvalidHeadOfDepartmentId_ShouldReturnError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var department = Department.Create(Guid.NewGuid(), "Geography", "Department of Geography", Guid.NewGuid()).Value;
        context.Departments.Add(department);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context);
        var command = new UpdateDepartmentCommand
        {
            Id = department.Id,
            Name = "Geography",
            Description = "Updated Department of Geography",
            HeadOfDepartmentId = Guid.NewGuid() 
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.UserNotFound, result.Errors.First());
    }
}
