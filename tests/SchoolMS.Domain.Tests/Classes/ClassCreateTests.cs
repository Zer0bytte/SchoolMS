using SchoolMS.Domain.Classes;

namespace SchoolMS.Domain.Tests.Classes;

public class ClassCreateTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Class A";
        var courseId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var semester = "Fall 2025";
        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 20);

        // Act
        var result = Class.Create(id, name, courseId, teacherId, semester, start, end);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(courseId, result.Value.CourseId);
        Assert.Equal(teacherId, result.Value.TeacherId);
        Assert.Equal(start, result.Value.StartDate);
        Assert.Equal(end, result.Value.EndDate);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNameIsEmpty()
    {
        var result = Class.Create(
            Guid.NewGuid(),
            "",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Fall",
            new DateOnly(2025, 1, 10),
            new DateOnly(2025, 1, 11));

        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.NameRequired, result.TopError);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCourseIdIsEmpty()
    {
        var result = Class.Create(
            Guid.NewGuid(),
            "Name",
            Guid.Empty,
            Guid.NewGuid(),
            "Fall",
            new DateOnly(2025, 1, 10),
            new DateOnly(2025, 1, 11));

        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.CourseIdInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenTeacherIdIsEmpty()
    {
        var result = Class.Create(
            Guid.NewGuid(),
            "Name",
            Guid.NewGuid(),
            Guid.Empty,
            "Fall",
            new DateOnly(2025, 1, 10),
            new DateOnly(2025, 1, 11));

        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.TeacherIdInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenSemesterIsEmpty()
    {
        var result = Class.Create(
            Guid.NewGuid(),
            "Name",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            new DateOnly(2025, 1, 10),
            new DateOnly(2025, 1, 11));

        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.SemesterRequired, result.TopError);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenStartDateIsDefault()
    {
        var result = Class.Create(
            Guid.NewGuid(),
            "Name",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Fall",
            default,
            new DateOnly(2025, 1, 11));

        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.StartDateInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenEndDateIsDefault()
    {
        var result = Class.Create(
            Guid.NewGuid(),
            "Name",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Fall",
            new DateOnly(2025, 1, 10),
            default);                     // ❌ invalid DateOnly

        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.EndDateInvalid, result.TopError);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenEndDateBeforeStartDate()
    {
        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 9);   // before start

        var result = Class.Create(
            Guid.NewGuid(),
            "Name",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Fall",
            start,
            end);

        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.EndDateBeforeStartDate, result.TopError);
    }
}