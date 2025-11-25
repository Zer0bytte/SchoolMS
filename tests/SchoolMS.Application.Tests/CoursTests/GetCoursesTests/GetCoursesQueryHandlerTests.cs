using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Features.Courses.Queries.GetCourses;
using SchoolMS.Application.Tests.Shared;

namespace SchoolMS.Application.Tests.CoursTests.GetCoursesTests;

public class GetCoursesQueryHandlerTests
{
    public Mock<ILogger<GetCoursesQueryHandler>> Logger { get; set; } = new();
    public HybridCache Cache { get; set; } = TestCacheHelper.CreateHybridCache();

    [Fact]
    public async Task Handle_ShouldReturnCourses()
    {
        await using var context = TestDbHelper.CreateContext();

        // seed data...
        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        var department = TestDbHelper.CreateDepartment(teacher);
        context.Departments.Add(department);
        context.Courses.AddRange(
            TestDbHelper.CreateCourse(department),
            TestDbHelper.CreateCourse(department));
        await context.SaveChangesAsync();

        // real HybridCache
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDistributedMemoryCache();
        services.AddHybridCache();

        using var sp = services.BuildServiceProvider();
        var cache = sp.GetRequiredService<HybridCache>();

        var handler = new GetCoursesQueryHandler(context, cache, Logger.Object);

        var result = await handler.Handle(new GetCoursesQuery { Limit = 10 }, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(2, result.Value.Items.Count);
    }


    [Fact]
    public async Task Handle_WithSearchTerm_ShouldReturnMatchingOnly()
    {
        // Arrange
        await using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        var department = TestDbHelper.CreateDepartment(teacher);
        context.Departments.Add(department);
        var course1 = TestDbHelper.CreateCourse(department);
        var course2 = TestDbHelper.CreateCourse(department);
        context.Courses.AddRange(course1, course2);
        await context.SaveChangesAsync();
        var handler = new GetCoursesQueryHandler(context, Cache, Logger.Object);
        var query = new GetCoursesQuery
        {
            Limit = 10,
            SearchTerm = course1.Name
        };
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);
        Assert.Equal(course1.Name, result.Value.Items[0].Name);
    }

    [Fact]
    public async Task Handle_WithDepartmentIdFilter_ShouldReturnMatchingOnly()
    {
        // Arrange
        await using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        var department1 = TestDbHelper.CreateDepartment(teacher, "Computer Science");
        var department2 = TestDbHelper.CreateDepartment(teacher, "Mathematics");
        context.Departments.AddRange(department1, department2);
        var course1 = TestDbHelper.CreateCourse(department1);
        var course2 = TestDbHelper.CreateCourse(department2);
        context.Courses.AddRange(course1, course2);
        await context.SaveChangesAsync();
        var handler = new GetCoursesQueryHandler(context, Cache, Logger.Object);
        var query = new GetCoursesQuery
        {
            Limit = 10,
            DepartmentId = department1.Id
        };
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Items);
        Assert.Equal(department1.Id, result.Value.Items[0].DepartmentId);
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        var department = TestDbHelper.CreateDepartment(teacher);
        context.Departments.Add(department);
        var courses = new List<Domain.Courses.Course>();
        for (int i = 0; i < 15; i++)
        {
            var course = TestDbHelper.CreateCourse(department);
            courses.Add(course);
            context.Courses.Add(course);
        }
        await context.SaveChangesAsync();
        var handler = new GetCoursesQueryHandler(context, Cache, Logger.Object);
        var firstPageQuery = new GetCoursesQuery { Limit = 10 };
        // Act - First Page
        var firstPageResult = await handler.Handle(firstPageQuery, CancellationToken.None);
        // Assert - First Page
        Assert.False(firstPageResult.IsError);
        Assert.NotNull(firstPageResult.Value);
        Assert.Equal(10, firstPageResult.Value.Items.Count);
        Assert.True(firstPageResult.Value.HasMore);
        Assert.NotNull(firstPageResult.Value.Cursor);
        // Act - Second Page
        var secondPageQuery = new GetCoursesQuery
        {
            Limit = 10,
            Cursor = firstPageResult.Value.Cursor
        };
        var secondPageResult = await handler.Handle(secondPageQuery, CancellationToken.None);
        // Assert - Second Page
        Assert.False(secondPageResult.IsError);
        Assert.NotNull(secondPageResult.Value);
        Assert.Equal(5, secondPageResult.Value.Items.Count);
        Assert.False(secondPageResult.Value.HasMore);
    }

    [Fact]
    public async Task Handle_InvalidCursor_ShouldReturnError()
    {
        // Arrange
        await using var context = TestDbHelper.CreateContext();
        var handler = new GetCoursesQueryHandler(context, Cache, Logger.Object);
        var query = new GetCoursesQuery
        {
            Cursor = "invalid_cursor_$$$",
            Limit = 10
        };
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_NoCourses_ShouldReturnEmptyList()
    {
        // Arrange
        await using var context = TestDbHelper.CreateContext();
        var handler = new GetCoursesQueryHandler(context, Cache, Logger.Object);
        var query = new GetCoursesQuery { Limit = 10 };
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Items);
        Assert.Null(result.Value.Cursor);
        Assert.False(result.Value.HasMore);
    }


    [Fact]
    public async Task Handle_SearchTermNoMatch_ShouldReturnEmptyList()
    {
        // Arrange
        await using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        context.Users.Add(teacher);
        var department = TestDbHelper.CreateDepartment(teacher);
        context.Departments.Add(department);
        var course = TestDbHelper.CreateCourse(department);
        context.Courses.Add(course);
        await context.SaveChangesAsync();
        var handler = new GetCoursesQueryHandler(context, Cache, Logger.Object);
        var query = new GetCoursesQuery
        {
            Limit = 10,
            SearchTerm = "NonExistentCourseName"
        };
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Items);
        Assert.Null(result.Value.Cursor);
        Assert.False(result.Value.HasMore);
    }

    [Fact]
    public async Task Handle_DepartmentIdNoMatch_ShouldReturnEmptyList()
    {
        // Arrange
        await using var context = TestDbHelper.CreateContext();
        var handler = new GetCoursesQueryHandler(context, Cache, Logger.Object);
        var query = new GetCoursesQuery
        {
            Limit = 10,
            DepartmentId = Guid.NewGuid()
        };
        // Act
        var result = await handler.Handle(query, CancellationToken.None);
        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Items);
        Assert.Null(result.Value.Cursor);
        Assert.False(result.Value.HasMore);
    }
}
