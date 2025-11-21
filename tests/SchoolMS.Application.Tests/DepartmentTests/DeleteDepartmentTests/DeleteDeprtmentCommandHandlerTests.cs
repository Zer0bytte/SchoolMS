using SchoolMS.Application.Features.Departments.Commands.DeleteDepartment;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Tests.DepartmentTests.DeleteDepartmentTests;

public class DeleteDeprtmentCommandHandlerTests
{


    [Fact]
    public async Task Handle_GivenValidDepartmentId_ShouldDeleteDepartment()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        var department = TestDbHelper.CreateDepartment(teacher);
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        var handler = new DeleteDepartmentCommandHandler(context);
        var command = new DeleteDepartmentCommand() { DepartmentId = department.Id };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        var deletedDepartment = await context.Departments.FindAsync(department.Id);
        Assert.Null(deletedDepartment);
    }

    [Fact]
    public async Task Handle_GivenInvalidDepartmentId_ShouldReturnNotFoundError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = TestDbHelper.CreateContext();
        var handler = new DeleteDepartmentCommandHandler(context);
        var command = new DeleteDepartmentCommand() { DepartmentId = Guid.NewGuid() };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DepartmentErrors.NotFound.Code, result.TopError.Code);
    }
}
