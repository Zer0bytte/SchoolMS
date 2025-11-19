using Microsoft.EntityFrameworkCore;
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

    public static User CreateTeacher()
    {
        var user = User.Create(Guid.NewGuid(), Guid.NewGuid().ToString(), "email@email.com", "haspass", Role.Teacher).Value;
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

}
