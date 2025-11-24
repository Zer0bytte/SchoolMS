using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Notifications.Commands;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Notifications.Enums;
using SchoolMS.Domain.StudentClasses;

namespace SchoolMS.Application.Tests.NotificationTests.SendNotification;

public class SendNotificationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenStudentDoesNotExist_ReturnsUserNotFound()
    {
        var context = TestDbHelper.CreateContext();

        var command = new SendNotificationCommand
        {
            IsClass = false,
            StudentId = Guid.NewGuid(),
            Title = "Hello",
            Message = "World"
        };

        var handler = new SendNotificationCommandHandler(context);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.UserNotFound, result.TopError);
    }

    [Fact]
    public async Task Handle_WhenStudentExists_CreatesNotificationSuccessfully()
    {
        //Arrange
        var context = TestDbHelper.CreateContext();
        var student = TestDbHelper.CreateStudent();
        context.Users.Add(student);
        await context.SaveChangesAsync();

        var command = new SendNotificationCommand
        {
            IsClass = false,
            StudentId = student.Id,
            Title = "Hello",
            Message = "World"
        };

        var handler = new SendNotificationCommandHandler(context);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        var dbNotification = context.Notifications.SingleOrDefault(x => x.RecipientId == student.Id);
        Assert.Equal(command.Title, dbNotification!.Title);
        Assert.Equal(command.Message, dbNotification.Message);
        Assert.True(result.IsSuccess);

    }

    [Fact]
    public async Task Handle_WhenClassDoesNotExist_ReturnsClassNotFound()
    {
        //Arrange
        var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);
        context.Users.Add(teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        await context.SaveChangesAsync();

        var command = new SendNotificationCommand
        {
            IsClass = true,
            ClassId = Guid.NewGuid(),
            Title = "Hello",
            Message = "World"
        };

        var handler = new SendNotificationCommandHandler(context);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.NotFound, result.TopError);
    }

    [Fact]
    public async Task Handle_WhenClassExists_CreatesNotificationsForAllStudents()
    {
        //Arrange
        var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);
        var student = TestDbHelper.CreateStudent();
        context.StudentClasses.Add(new StudentClass
        {
            ClassId = cls.Id,
            EnrollmentDate = DateTime.UtcNow,
            StudentId = student.Id,
        });
        context.StudentClasses.Add(new StudentClass
        {
            ClassId = cls.Id,
            EnrollmentDate = DateTime.UtcNow,
            StudentId = student.Id,
        });
        context.Users.Add(teacher);
        context.Users.Add(student);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        await context.SaveChangesAsync();

        var command = new SendNotificationCommand
        {
            IsClass = true,
            ClassId = cls.Id,
            Title = "Hello",
            Message = "World"
        };

        var handler = new SendNotificationCommandHandler(context);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);

        var dbNotification = await context.Notifications.Where(x => x.RecipientRole == RecipientRole.Class).ToListAsync();
        Assert.Equal(2, dbNotification.Count);
    }
}
