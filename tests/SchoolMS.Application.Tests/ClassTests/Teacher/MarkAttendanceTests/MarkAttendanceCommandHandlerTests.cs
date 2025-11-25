using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Classes.Commands.MarkAttendance;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Application.Features.Classes.Queries.Teacher.GetClassAttendance;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Attendances.Enums;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.StudentClasses;

namespace SchoolMS.Application.Tests.ClassTests.Teacher.MarkAttendanceTests;

public class MarkAttendanceCommandHandlerTests
{
    public Mock<ILogger<MarkAttendanceCommandHandler>> Logger { get; set; } = new();

    [Fact]
    public async Task Handle_WithOneAttendanceRecord_ShouldMarkAttendance()
    {
        //Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var student = TestDbHelper.CreateStudent();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);

        context.Users.Add(student);
        context.Users.Add(teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        context.StudentClasses.Add(new StudentClass
        {
            ClassId = cls.Id,
            StudentId = student.Id,
            EnrollmentDate = DateTime.Now,
        });
        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var command = new MarkAttendanceCommand
        {
            ClassId = cls.Id,
            Students =
            {
                new StudentAttendanceEntry
                {
                    StudentId = student.Id,
                    Status = AttendanceStatus.Present,
                }
            }
        };

        var handler = new MarkAttendanceCommandHandler(context, user.Object, Logger.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);


        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(Result.Success, result.Value);

        var attendanceCount = await context.Attendances.CountAsync(a => a.ClassId == cls.Id);
        Assert.Equal(1, attendanceCount);
    }


    [Fact]
    public async Task Handle_WithOnlyOneValidStudentId_ShouldReturnError()
    {
        //Arrange
        var user = new Mock<IUser>();

        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);
        var student = TestDbHelper.CreateStudent();
        context.Users.Add(teacher);
        context.Users.Add(student);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var command = new MarkAttendanceCommand
        {
            ClassId = cls.Id,
            Students = {
                new StudentAttendanceEntry
                {
                    StudentId = student.Id,
                    Status = AttendanceStatus.Present,
                },
                new StudentAttendanceEntry
                {
                    StudentId = Guid.NewGuid(),
                    Status = AttendanceStatus.Present,
                }
            }
        };

        var handler = new MarkAttendanceCommandHandler(context, user.Object, Logger.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsError);

        Assert.Equal("Students.NotFound", result.TopError.Code);
    }


    [Fact]
    public async Task Handle_InvalidClassId_ShouldReturnClassNotFound()
    {
        var user = new Mock<IUser>();

        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var command = new MarkAttendanceCommand
        {
            ClassId = Guid.NewGuid(),
            Students = {
                new StudentAttendanceEntry
                {
                    StudentId = Guid.NewGuid(),
                    Status = AttendanceStatus.Present,
                },
                new StudentAttendanceEntry
                {
                    StudentId = Guid.NewGuid(),
                    Status = AttendanceStatus.Present,
                }
            }
        };

        var handler = new MarkAttendanceCommandHandler(context, user.Object, Logger.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsError);

        Assert.Equal(ClassErrors.NotFound, result.TopError);
    }
}
