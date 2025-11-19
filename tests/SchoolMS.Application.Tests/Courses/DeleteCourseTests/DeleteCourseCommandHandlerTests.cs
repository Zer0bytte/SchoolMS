using SchoolMS.Application.Features.Courses.Commands.DeleteCourse;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Tests.Courses.DeleteCourse;

public class DeleteCourseCommandHandlerTests
{
    [Fact]
    public async Task HandleGivenValidCourseId_ShouldDeleteCourse()
    {
        // Arrange
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);

        context.Users.Add(teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);

        await context.SaveChangesAsync();

        var handler = new DeleteCourseCommandHandler(context);
        var command = new DeleteCourseCommand() { Id = course.Id };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        var deletedCourse = await context.Courses.FindAsync(course.Id);
        Assert.Equal(deletedCourse.IsActive, false);
    }

    [Fact]
    public async Task HandleGivenInvalidCourseId_ShouldReturnNotFoundError()
    {
        // Arrange
        using var context = TestDbHelper.CreateContext();
        var handler = new DeleteCourseCommandHandler(context);
        var command = new DeleteCourseCommand() { Id = Guid.NewGuid() };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.NotFound, result.TopError);
    }
}
