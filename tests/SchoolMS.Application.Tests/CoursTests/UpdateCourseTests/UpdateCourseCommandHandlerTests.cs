using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Features.Courses.Commands.CreateCourse;
using SchoolMS.Application.Features.Courses.Commands.UpdateCourse;
using SchoolMS.Application.Tests.Shared;

namespace SchoolMS.Application.Tests.CoursTests.UpdateCourseTests;

public class UpdateCourseCommandHandlerTests
{
    public Mock<ILogger<UpdateCourseCommandHandler>> Logger { get; set; } = new();
    public Mock<HybridCache> Cache { get; set; } = new();

    [Fact]
    public  async Task Handle_GivenValidRequest_ShouldUpdateCourse()
    {
        //Arrange
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var department2 = TestDbHelper.CreateDepartment(teacher);
        context.Users.Add(teacher);
        context.Departments.Add(department);
        await context.SaveChangesAsync();

        var course1 = TestDbHelper.CreateCourse(department);
        context.Courses.Add(course1);


        await context.SaveChangesAsync();


        var handler = new UpdateCourseCommandHandler(context, Logger.Object, Cache.Object);
        var command = new UpdateCourseCommand
        {
            Id = course1.Id,
            Name = "Updated Course Name",
            Description = "Updated Course Description",
            Credits = 4,
            Code = Guid.NewGuid().ToString(),
            DepartmentId = department2.Id
        };

        //Act

        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);

        var updatedCourse = context.Courses.First(c => c.Id == course1.Id);
        Assert.Equal("Updated Course Name", updatedCourse.Name);
        Assert.Equal("Updated Course Description", updatedCourse.Description);
        Assert.Equal(4, updatedCourse.Credits);
        Assert.Equal(command.Code, updatedCourse.Code);
        Assert.Equal(department2.Id, updatedCourse.DepartmentId);

    }
}
