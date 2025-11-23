using Microsoft.EntityFrameworkCore;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Classes.Commands.DeactivateClass;
using SchoolMS.Application.Tests.Shared;

namespace SchoolMS.Application.Tests.ClassTests.DeactivateClassTests;

public class DeactivateClassCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidParams_ShouldDeactivateClass()
    {
        //Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);
        context.Users.Add(teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var command = new DeactivateClassCommand
        {
            Id = cls.Id
        };

        var handler = new DeactivateClassCommandHandler(context, user.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);

        Assert.True(result.IsSuccess);
        var dbClass = await context.Classes.FirstOrDefaultAsync(cls => cls.Id == cls.Id);
        Assert.False(dbClass!.IsActive);
    }
}
