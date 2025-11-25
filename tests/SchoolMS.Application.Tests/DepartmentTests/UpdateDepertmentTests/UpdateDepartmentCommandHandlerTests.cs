using Castle.Core.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Tests.DepartmentTests.UpdateDepertmentTests;

public class UpdateDepartmentCommandHandlerTests
{
    public Mock<ILogger<UpdateDepartmentCommandHandler>> Logger { get; set; } = new();
    public Mock<HybridCache> Cache { get; set; } = new();
    [Fact]
    public async Task Handle_GivenValidRequest_ShouldUpdateDepartment()
    {
        // Arrange
        await using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = Department.Create(Guid.NewGuid(), "Mathematics", "Department of Mathematics", teacher.Id).Value;

        context.Departments.Add(department);
        context.Users.Add(teacher);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context, Logger.Object, Cache.Object);
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
        await using var context = TestDbHelper.CreateContext();
        var handler = new UpdateDepartmentCommandHandler(context, Logger.Object, Cache.Object);
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
        await using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department1 = Department.Create(Guid.NewGuid(), "Biology", "Department of Biology", teacher.Id).Value;
        var department2 = Department.Create(Guid.NewGuid(), "Chemistry", "Department of Chemistry", teacher.Id).Value;
        context.Departments.Add(department1);
        context.Departments.Add(department2);
        context.Users.Add(teacher);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context, Logger.Object, Cache.Object);
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
        await using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = Department.Create(Guid.NewGuid(), "History", "Department of History", teacher.Id).Value;
        context.Departments.Add(department);
        context.Users.Add(teacher);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context, Logger.Object, Cache.Object);
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
        await using var context = TestDbHelper.CreateContext();
        var department = Department.Create(Guid.NewGuid(), "Geography", "Department of Geography", Guid.NewGuid()).Value;
        context.Departments.Add(department);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context, Logger.Object, Cache.Object);
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


    [Fact]
    public async Task Handle_GivenValidHeadOfDepartmentId_ShouldUpdateDepartment_RetrunNewHeadName()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();
        var teacher1 = TestDbHelper.CreateTeacher();
        var teacher2 = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher1);
        context.Users.Add(teacher2);
        var department = Department.Create(Guid.NewGuid(), "English", "Department of English", teacher1.Id).Value;
        context.Departments.Add(department);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context, Logger.Object, Cache.Object);
        var command = new UpdateDepartmentCommand
        {
            Id = department.Id,
            Name = "English",
            Description = "Updated Department of English",
            HeadOfDepartmentId = teacher2.Id
        };


        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.NotNull(result.Value);
        Assert.Equal(result.Value.HeadOfDepartmentName, teacher2.Name);
    }

    [Fact]
    public async Task Handle_GivenHeadOfDepartmentIdIsNotATeacher_ShouldReturnError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();
        var student = TestDbHelper.CreateStudent();
        context.Users.Add(student);
        var department = Department.Create(Guid.NewGuid(), "Art", "Department of Art", Guid.NewGuid()).Value;
        context.Departments.Add(department);
        await context.SaveChangesAsync();
        var handler = new UpdateDepartmentCommandHandler(context, Logger.Object, Cache.Object);
        var command = new UpdateDepartmentCommand
        {
            Id = department.Id,
            Name = "Art",
            Description = "Updated Department of Art",
            HeadOfDepartmentId = student.Id
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.HeadOfDepartmentShouldBeTeacher, result.TopError);
    }

}
