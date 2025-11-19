using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Features.Departments.Commands.CreateDepartment;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Tests.Departments.CreateDepartmentTests;

public class CreateDepartmentCommandHandlerTests
{
    
    [Fact]
    public async Task Handle_GivenValidRequest_ShouldCreateDepartment()
    {
        // Arrange

        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();
        var teacher = User.Create(Guid.NewGuid(), "Alice Smith", "email@email.com", "SecurePass123", Role.Teacher).Value;
        context.Users.Add(teacher);
        await context.SaveChangesAsync();


        var handler = new CreateDepartmentCommandHandler(context);
        var command = new CreateDepartmentCommand
        {
            Name = "Computer Science",
            Description = "Department of Computer Science",
            HeadOfDepartmentId = teacher.Id
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Computer Science", result.Value.Name);
        Assert.Equal("Department of Computer Science", result.Value.Description);
        var departmentInDb = await context.Departments.FindAsync(result.Value.Id);

        Assert.NotNull(departmentInDb);

        Assert.Equal("Computer Science", departmentInDb!.Name);
        Assert.Equal("Department of Computer Science", departmentInDb.Description);
    }

    [Fact]
    public void Handle_GivenInvalidHeadOfDepartmentId_ShouldReturnError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbHelper.CreateContext();
        var handler = new CreateDepartmentCommandHandler(context);
        var command = new CreateDepartmentCommand
        {
            Name = "Mathematics",
            Description = "Department of Mathematics",
            HeadOfDepartmentId = Guid.NewGuid()
        };

        // Act
        var result = handler.Handle(command, CancellationToken.None).Result;

        // Assert
        Assert.True(result.IsError);
        Assert.True(result.Errors.Any(e => e.Code == "User.NotFound"));
    }


    [Fact]
    public async Task Handle_GivenDuplicateDepartmentName_ShouldReturnError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();
        var existingDepartment = SchoolMS.Domain.Departments.Department.Create(
            Guid.NewGuid(),
            "Physics",
            "Department of Physics",
            Guid.NewGuid()
        ).Value;
        context.Departments.Add(existingDepartment);
        await context.SaveChangesAsync();
        var handler = new CreateDepartmentCommandHandler(context);
        var command = new CreateDepartmentCommand
        {
            Name = "Physics", 
            Description = "Another Department of Physics",
            HeadOfDepartmentId = Guid.NewGuid()
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.True(result.Errors.Any(e => e.Code == "Department.Name.Exists"));
    }
}
