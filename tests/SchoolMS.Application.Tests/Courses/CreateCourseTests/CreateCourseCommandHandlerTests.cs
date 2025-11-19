using SchoolMS.Application.Features.Courses.Commands.CreateCourse;
using SchoolMS.Application.Tests.Shared;

namespace SchoolMS.Application.Tests.Courses.CreateCourse;

public class CreateCourseCommandHandlerTests
{

    [Fact]
    public async Task Handle_GivenValidRequest_ShouldCreateCourse()
    {
        //Arrange
        using var dbContext = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);

        dbContext.Users.Add(teacher);
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync();

        var handler = new CreateCourseCommandHandler(dbContext);
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Programming",
            Description = "A beginner course on programming concepts.",
            DepartmentId = department.Id,
            Code = "CS101",
            Credits = 3
        };

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(command.Description, result.Value.Description);
        Assert.Equal(command.DepartmentId, result.Value.DepartmentId);
        Assert.Equal(command.Code, result.Value.Code);
        Assert.Equal(command.Credits, result.Value.Credits);
    }
}
