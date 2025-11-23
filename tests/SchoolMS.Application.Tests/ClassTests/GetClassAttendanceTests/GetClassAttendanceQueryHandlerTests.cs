using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Classes.Commands.MarkAttendance;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Application.Features.Classes.Queries.GetClassAttendance;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Attendances.Enums;

namespace SchoolMS.Application.Tests.ClassTests.GetClassAttendanceTests;

public class GetClassAttendanceQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetClassAttendance_ShouldReturnAttendance()
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
        cls.StudentClasses.Add(new Domain.StudentClasses.StudentClass
        {
            ClassId = cls.Id,
            EnrollmentDate = new DateTime(2025, 1, 1),
            StudentId = student.Id
        });
        context.Classes.Add(cls);
        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var cmdMarkAttendance = new MarkAttendanceCommand
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

        var markHandler = new MarkAttendanceCommandHandler(context, user.Object);
        await markHandler.Handle(cmdMarkAttendance, CancellationToken.None);

        var query = new GetClassAttendanceQuery
        {
            ClassId = cls.Id
        };

        var handler = new GetClassAttendanceQueryHandler(context, user.Object);

        //Act

        var result = await handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.Contains(result.Value, s => s.StudentId == student.Id);

    }
}
