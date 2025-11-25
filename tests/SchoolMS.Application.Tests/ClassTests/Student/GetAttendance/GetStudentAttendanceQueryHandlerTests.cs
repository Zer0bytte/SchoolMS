using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Classes.Queries.Student.GetStudentAttendance;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Attendances.Enums;

namespace SchoolMS.Application.Tests.ClassTests.Student.GetAttendance;

public class GetStudentAttendanceQueryHandlerTests
{
    public Mock<ILogger<GetStudentAttendanceQueryHandler>> Logger { get; set; } = new();
    [Fact]
    public async Task Handle_WhenStudentHasAttendance_ShouldReturnStudentAttendance()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        var student = TestDbHelper.CreateStudent();
        var anotherStudent = TestDbHelper.CreateStudent();

        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);

        var cls1 = TestDbHelper.CreateClass(course, teacher);
        var cls2 = TestDbHelper.CreateClass(course, teacher);
        var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

        var attendance1 = Attendance.Create(Guid.NewGuid(), cls1.Id, student.Id, dateNow, AttendanceStatus.Present, teacher.Id);
        var attendance2 = Attendance.Create(Guid.NewGuid(), cls2.Id, student.Id, dateNow, AttendanceStatus.Absent, teacher.Id);
        var attendance3 = Attendance.Create(Guid.NewGuid(), cls1.Id, anotherStudent.Id, dateNow, AttendanceStatus.Present, teacher.Id);
        context.Attendances.Add(attendance1.Value);
        context.Attendances.Add(attendance2.Value);
        context.Attendances.Add(attendance3.Value);
        context.Classes.AddRange(cls1, cls2);
        context.Users.AddRange(student, anotherStudent, teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);

        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var handler = new GetStudentAttendanceQueryHandler(context, user.Object, Logger.Object);

        var query = new GetStudentAttendanceQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Count);


        Assert.Contains(result.Value, x => x.ClassId == cls1.Id && x.Status == AttendanceStatus.Present);
        Assert.Contains(result.Value, x => x.ClassId == cls2.Id && x.Status == AttendanceStatus.Absent);
    }


    [Fact]
    public async Task Handle_WhenStudentHasNoAttendance_ShouldReturnEmptyList()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();

        var student = TestDbHelper.CreateStudent();
        context.Users.Add(student);
        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var handler = new GetStudentAttendanceQueryHandler(context, user.Object, Logger.Object);

        // Act
        var result = await handler.Handle(new GetStudentAttendanceQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectDtoFields()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        var student = TestDbHelper.CreateStudent();

        var dept = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(dept);
        var cls = TestDbHelper.CreateClass(course, teacher);

        var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

        var attendance = Attendance.Create(Guid.NewGuid(), cls.Id, student.Id, dateNow, AttendanceStatus.Late, teacher.Id);

        context.Attendances.Add(attendance.Value);
        context.Classes.Add(cls);
        context.Users.AddRange(student, teacher);
        context.Departments.Add(dept);
        context.Courses.Add(course);

        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var handler = new GetStudentAttendanceQueryHandler(context, user.Object, Logger.Object);

        // Act
        var result = await handler.Handle(new GetStudentAttendanceQuery(), CancellationToken.None);

        // Assert
        var dto = Assert.Single(result.Value);

        Assert.Equal(attendance.Value.ClassId, dto.ClassId);
        Assert.Equal(cls.Name, dto.ClassName);
        Assert.Equal(attendance.Value.Status, dto.Status);
    }

}
