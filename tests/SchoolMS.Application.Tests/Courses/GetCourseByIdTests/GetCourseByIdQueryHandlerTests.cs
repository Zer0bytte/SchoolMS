using SchoolMS.Application.Features.Courses.Queries.GetCourseById;
using SchoolMS.Application.Tests.Shared;

namespace SchoolMS.Application.Tests.Courses.GetCourseByIdTests;

public class GetCourseByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnCourse_WhenCourseExists()
    {
        //Arrange
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        context.Users.Add(teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);

        await context.SaveChangesAsync();

        var handler = new GetCourseByIdQueryHandler(context);

        var query = new GetCourseByIdQuery
        {
            Id = course.Id
        };

        //Act
        var result = await handler.Handle(query, CancellationToken.None);


        //Assert
        Assert.NotNull(result.Value);
        Assert.Equal(course.Id, result.Value.Id);
        Assert.Equal(course.Name, result.Value.Name);
        Assert.Equal(course.Description, result.Value.Description);
        Assert.Equal(course.Code, result.Value.Code);
        Assert.Equal(course.Credits, result.Value.Credits);
        Assert.Equal(course.DepartmentId, result.Value.DepartmentId);
        Assert.Equal(course.Department.Name, result.Value.DepartmentName);
    }

}
