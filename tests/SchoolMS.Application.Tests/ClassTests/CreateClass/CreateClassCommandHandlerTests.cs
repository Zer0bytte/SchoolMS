using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Classes.Commands.CreateClass;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Tests.ClassTests.CreateClass;

public class CreateClassCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidParams_ShouldCreateClass()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        context.Users.Add(teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        user.Setup(u => u.Role).Returns("Teacher");
        var handler = new CreateClassCommandHandler(context, user.Object);

        var command = new CreateClassCommand
        {
            CourseId = course.Id,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(4)),
            Name = "Class A",
            Semester = "Fall 2024"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }



    [Fact]
    public async Task Handle_WithInvalidCourseId_ShouldReturnError()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        user.Setup(u => u.Role).Returns("Teacher");
        var handler = new CreateClassCommandHandler(context, user.Object);
        var command = new CreateClassCommand
        {
            CourseId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(4)),
            Name = "Class A",
            Semester = "Fall 2024"
        };
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(result.TopError, CourseErrors.NotFound);
    }
}