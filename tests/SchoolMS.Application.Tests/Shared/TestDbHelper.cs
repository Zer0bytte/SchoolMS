using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Tests.Shared;

public class TestDbHelper
{
    public static TestAppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestAppDbContext(options);
    }
    public static User CreateAdmin()
    {
        var user = User.Create(Guid.NewGuid(), Guid.NewGuid().ToString(), "email@email.com", "haspass", Role.Admin).Value;
        return user;
    }
    public static User CreateTeacher()
    {
        var user = User.Create(Guid.NewGuid(), Guid.NewGuid().ToString(), "email@email.com", "haspass", Role.Teacher).Value;
        return user;
    }

    public static User CreateStudent()
    {
        var user = User.Create(Guid.NewGuid(), Guid.NewGuid().ToString(), "email@email.com", "haspass", Role.Student).Value;
        return user;
    }

    public static Department CreateDepartment(User teacher, string departmentName = "Computer Science")
    {
        var departmentResult = Department.Create(
            Guid.NewGuid(),
            departmentName,
            "Department of Computer Science",
            teacher.Id
        );
        return departmentResult.Value;
    }

    public static Course CreateCourse(Department department)
    {
        var courseResult = Course.Create(
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "Department of Computer Science",
            department.Id,
            1
        );

        return courseResult.Value;
    }
    public static Class CreateClass(Course course, User teacher, string className = "clsName")
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        var start = now.AddDays(1);
        var end = now.AddDays(30);
        return Class.Create(Guid.NewGuid(), className, course.Id, teacher.Id, Guid.NewGuid().ToString(), start, end).Value;
    }

    public static Assignment CreateAssignment(Class cls, User teacher)
    {
        var dueDate = new DateOnly(2025, 12, 20).AddDays(10);

        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        return Assignment.Create(Guid.NewGuid(), cls.Id, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), dueDate, teacher.Id, now).Value;
    }
}