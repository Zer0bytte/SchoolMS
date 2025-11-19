using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Tests.Departments.DeleteDepartmentTests;

public class DeleteDeprtmentCommandHandlerTests
{

    private TestAppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestAppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new TestAppDbContext(options);
    }

    private User CreateTeacher()
    {
        var user = User.Create(Guid.NewGuid(), "John Doe", "email@email.com", "haspass", Role.Teacher).Value;
        return user;
    }
    private Department CreateDepartment()
    {
        var departmentResult = Department.Create(
            Guid.NewGuid(),
            "Computer Science",
            "Department of Computer Science",
            Guid.NewGuid()
        );
        return departmentResult.Value;
    }
}
