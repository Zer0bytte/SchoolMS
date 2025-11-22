using Microsoft.EntityFrameworkCore;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Classes.Commands.UpdateClass;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Tests.ClassTests.UpdateClass;

public class UpdateClassCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidParams_ShouldUpdateClass()
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

        var command = new UpdateClassCommand
        {
            Id = cls.Id,
            Name = "Changed Name",
            EndDate = cls.EndDate,
            StartDate = cls.StartDate,
            Semester = cls.Semester,
        };

        var handler = new UpdateClassCommandHandler(context, user.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        var dbClass = await context.Classes.FirstOrDefaultAsync(cls => cls.Id == cls.Id);
        Assert.NotNull(dbClass);
        Assert.Equal(dbClass.Name, command.Name);
        Assert.Equal(dbClass.EndDate, command.EndDate);
        Assert.Equal(dbClass.StartDate, command.StartDate);
        Assert.Equal(dbClass.Semester, command.Semester);
    }

    [Fact]
    public async Task Handle_InvalidClassId_ShouldReturnClassNotFound()
    {
        //Arrange
        using var context = TestDbHelper.CreateContext();
        var user = new Mock<IUser>();
        user.Setup(u => u.Id).Returns(Guid.NewGuid().ToString());

        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 20);
        var command = new UpdateClassCommand
        {
            Id = Guid.NewGuid(),
            Name = "Changed Name",
            StartDate = start,
            EndDate = end,
            Semester = Guid.NewGuid().ToString(),
        };

        var handler = new UpdateClassCommandHandler(context, user.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsError);
        Assert.Equal(result.TopError, ClassErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnDuplicateNameError_WhenClassNameAlreadyExists()
    {
        // Arrange
        var context = TestDbHelper.CreateContext();
        var user = new Mock<IUser>();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher, "Class 1");
        var cls2 = TestDbHelper.CreateClass(course, teacher, "Class 2");
        context.Users.Add(teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        context.Classes.Add(cls2);
        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var command = new UpdateClassCommand
        {
            Id = cls.Id,
            Name = cls2.Name,
        };
        var handler = new UpdateClassCommandHandler(context, user.Object);
        // Act

        var result = await handler.Handle(command, CancellationToken.None);

        // Assert

        Assert.NotNull(result);
        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.DublicateName, result.TopError);
    }


}
