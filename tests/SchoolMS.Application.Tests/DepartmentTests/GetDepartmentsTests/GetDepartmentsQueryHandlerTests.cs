using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Departments.Commands.CreateDepartment;
using SchoolMS.Application.Features.Departments.Queries.GetDepartments;
using SchoolMS.Application.Tests.Shared;

namespace SchoolMS.Application.Tests.DepartmentTests.GetDepartmentsTests;

public class GetDepartmentsQueryHandlerTests
{
    public Mock<ILogger<GetDepartmentsQueryHandler>> Logger { get; set; } = new();
    public HybridCache Cache { get; set; } = TestCacheHelper.CreateHybridCache();


    [Fact]
    public async Task Handle_WhenUserIsAdmin_ShouldReturnAllDepartments()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        var teacher2 = TestDbHelper.CreateTeacher();
        var admin = TestDbHelper.CreateAdmin();
        context.Users.Add(teacher);
        context.Users.Add(teacher2);
        var dept1 = TestDbHelper.CreateDepartment(teacher);
        var dept2 = TestDbHelper.CreateDepartment(teacher2);

        context.Departments.AddRange(dept1, dept2);
        await context.SaveChangesAsync();
        var user = new Mock<IUser>();

        user.Setup(u => u.Id).Returns(admin.Id.ToString());

        var handler = new GetDepartmentsQueryHandler(context, user.Object, Logger.Object, Cache);
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
    public async Task Handle_WhenUserIsTeacher_ShouldReturnTeacherDepartments()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        var teacher2 = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        context.Users.Add(teacher2);
        var dept1 = TestDbHelper.CreateDepartment(teacher);
        var dept2 = TestDbHelper.CreateDepartment(teacher2);

        context.Departments.AddRange(dept1, dept2);
        await context.SaveChangesAsync();
        var user = new Mock<IUser>();

        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var handler = new GetDepartmentsQueryHandler(context, user.Object, Logger.Object, Cache);
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
        await using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);

        var dept1 = TestDbHelper.CreateDepartment(teacher, "Computer Science");
        var dept2 = TestDbHelper.CreateDepartment(teacher, "Information technology");

        context.Departments.AddRange(dept1, dept2);
        await context.SaveChangesAsync();
        var user = new Mock<IUser>();

        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var handler = new GetDepartmentsQueryHandler(context, user.Object, Logger.Object, Cache);
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
        await using var context = TestDbHelper.CreateContext();

        var user = new Mock<IUser>();

        user.Setup(u => u.Id).Returns(Guid.NewGuid().ToString());

        var handler = new GetDepartmentsQueryHandler(context, user.Object, Logger.Object, Cache);
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
        await using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);

        var now = DateTimeOffset.UtcNow;

        var dept1 = TestDbHelper.CreateDepartment(teacher);
        var dept2 = TestDbHelper.CreateDepartment(teacher);
        var dept3 = TestDbHelper.CreateDepartment(teacher);

        context.Departments.AddRange(dept1, dept2, dept3);
        await context.SaveChangesAsync();

        var user = new Mock<IUser>();

        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var handler = new GetDepartmentsQueryHandler(context, user.Object, Logger.Object, Cache);
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