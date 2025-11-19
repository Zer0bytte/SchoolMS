using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Features.Departments.Queries.GetDepartments;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Tests.Departments.GetDepartmentsTests;

public class GetDepartmentsQueryHandlerTests
{
    private TestAppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestAppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new TestAppDbContext(options);
    }

    private User CreateTeacher(string name = "Teacher")
    {
        return User.Create(
            Guid.NewGuid(),
            name,
            $"{Guid.NewGuid()}@email.com",
            "SecurePass123",
            Role.Teacher
        ).Value;
    }

    private Department CreateDepartment(string name, string desc, Guid headId, DateTimeOffset created)
    {
        var department = Department.Create(
            Guid.NewGuid(),
            name,
            desc,
            headId
        ).Value;

        department.CreatedDateUtc = created;
        return department;
    }

    [Fact]
    public async Task Handle_ShouldReturnDepartments()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var teacher = CreateTeacher();
        context.Users.Add(teacher);

        var dept1 = CreateDepartment("CS", "Computer Science", teacher.Id, DateTimeOffset.UtcNow.AddHours(-2));
        var dept2 = CreateDepartment("Math", "Mathematics", teacher.Id, DateTimeOffset.UtcNow.AddHours(-1));

        context.Departments.AddRange(dept1, dept2);
        await context.SaveChangesAsync();

        var handler = new GetDepartmentsQueryHandler(context);
        var query = new GetDepartmentsQuery { Limit = 10 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Null(result.Value.Cursor);
        Assert.False(result.Value.HasMore);
    }

    [Fact]
    public async Task Handle_WithNameFilter_ShouldReturnMatchingOnly()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var teacher = CreateTeacher();
        context.Users.Add(teacher);

        var dept1 = CreateDepartment("Computer Science", "CS Dept", teacher.Id, DateTimeOffset.UtcNow);
        var dept2 = CreateDepartment("History", "History Dept", teacher.Id, DateTimeOffset.UtcNow);

        context.Departments.AddRange(dept1, dept2);
        await context.SaveChangesAsync();

        var handler = new GetDepartmentsQueryHandler(context);
        var query = new GetDepartmentsQuery
        {
            DepartmentName = "Comp",
            Limit = 10
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Single(result.Value.Items);
        Assert.Equal("Computer Science", result.Value.Items[0].Name);
    }

    [Fact]
    public async Task Handle_InvalidCursor_ShouldReturnError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var handler = new GetDepartmentsQueryHandler(context);
        var query = new GetDepartmentsQuery
        {
            Cursor = "invalid_cursor_$$$",
            Limit = 10
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Code == "InvalidCursor");
    }

    [Fact]
    public async Task Handle_ShouldReturnNextCursor_WhenHasMoreResults()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var teacher = CreateTeacher();
        context.Users.Add(teacher);

        var now = DateTimeOffset.UtcNow;

        var dept1 = CreateDepartment("Dept A", "A", teacher.Id, now.AddMinutes(-1));
        var dept2 = CreateDepartment("Dept B", "B", teacher.Id, now.AddMinutes(-2));
        var dept3 = CreateDepartment("Dept C", "C", teacher.Id, now.AddMinutes(-3));

        context.Departments.AddRange(dept1, dept2, dept3);
        await context.SaveChangesAsync();

        var handler = new GetDepartmentsQueryHandler(context);

        var query = new GetDepartmentsQuery
        {
            Limit = 2 // expect only first 2, with a next cursor for the 3rd
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.True(result.Value.HasMore);
        Assert.NotNull(result.Value.Cursor); 
    }
}