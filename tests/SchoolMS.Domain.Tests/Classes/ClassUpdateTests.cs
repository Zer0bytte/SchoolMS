using SchoolMS.Domain.Classes;

namespace SchoolMS.Domain.Tests.Classes;

public class ClassUpdateTests
{
    private readonly DateOnly _validStartDate = new(2025, 1, 10);
    private readonly DateOnly _validEndDate = new(2025, 1, 20);

    private Class CreateValidClass()
    {
        return Class.Create(
            Guid.NewGuid(),
            "Original Name",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Fall 2025",
            _validStartDate,
            _validEndDate
        ).Value;
    }

    [Fact]
    public void Update_ValidParams_ShouldUpdateAllFields()
    {
        // Arrange
        var cls = CreateValidClass();
        var newStart = new DateOnly(2025, 2, 1);
        var newEnd = new DateOnly(2025, 2, 28);

        // Act
        var result = cls.Update("New Name", "Spring 2025", newStart, newEnd);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Value.Name);
        Assert.Equal("Spring 2025", result.Value.Semester);
        Assert.Equal(newStart, result.Value.StartDate);
        Assert.Equal(newEnd, result.Value.EndDate);
    }

    [Fact]
    public void Update_NullParams_ShouldNotChangeAnyFields()
    {
        // Arrange
        var cls = CreateValidClass();
        var originalName = cls.Name;
        var originalSemester = cls.Semester;

        // Act
        var result = cls.Update(null, null, null, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(originalName, result.Value.Name);
        Assert.Equal(originalSemester, result.Value.Semester);
        Assert.Equal(_validStartDate, result.Value.StartDate);
        Assert.Equal(_validEndDate, result.Value.EndDate);
    }

    [Fact]
    public void Update_NameWithLeadingTrailingWhitespace_ShouldTrim()
    {
        // Arrange
        var cls = CreateValidClass();

        // Act
        var result = cls.Update("  Trimmed Name  ", null, null, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Trimmed Name", result.Value.Name);
    }

    [Fact]
    public void Update_SemesterWithLeadingTrailingWhitespace_ShouldTrim()
    {
        // Arrange
        var cls = CreateValidClass();

        // Act
        var result = cls.Update(null, "  Spring 2025  ", null, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Spring 2025", result.Value.Semester);
    }

    [Fact]
    public void Update_EndDateBeforeStartDate_ShouldReturnError()
    {
        // Arrange
        var cls = CreateValidClass();
        var newStart = new DateOnly(2025, 2, 20);
        var newEnd = new DateOnly(2025, 2, 10);

        // Act
        var result = cls.Update("New Name", "Spring 2025", newStart, newEnd);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.EndDateBeforeStartDate, result.TopError);
    }

    [Fact]
    public void Update_NewEndDateBeforeCurrentStartDate_ShouldReturnError()
    {
        // Arrange
        var cls = CreateValidClass();
        var newEnd = new DateOnly(2025, 1, 5); // Before current start date

        // Act
        var result = cls.Update(null, null, null, newEnd);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.EndDateBeforeStartDate, result.TopError);
    }

    [Fact]
    public void Update_NewStartDateAfterCurrentEndDate_ShouldReturnError()
    {
        // Arrange
        var cls = CreateValidClass();
        var newStart = new DateOnly(2025, 1, 25); // After current end date

        // Act
        var result = cls.Update(null, null, newStart, null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.EndDateBeforeStartDate, result.TopError);
    }

    [Fact]
    public void Update_OnlyStartDate_ShouldUpdateStartDateOnly()
    {
        // Arrange
        var cls = CreateValidClass();
        var newStart = new DateOnly(2025, 1, 15);

        // Act
        var result = cls.Update(null, null, newStart, null);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newStart, result.Value.StartDate);
        Assert.Equal(_validEndDate, result.Value.EndDate);
    }

    [Fact]
    public void Update_OnlyEndDate_ShouldUpdateEndDateOnly()
    {
        // Arrange
        var cls = CreateValidClass();
        var newEnd = new DateOnly(2025, 1, 15);

        // Act
        var result = cls.Update(null, null, null, newEnd);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_validStartDate, result.Value.StartDate);
        Assert.Equal(newEnd, result.Value.EndDate);
    }

    [Fact]
    public void Update_SameDatesForStartAndEnd_ShouldSucceed()
    {
        // Arrange
        var cls = CreateValidClass();
        var sameDate = new DateOnly(2025, 1, 15);

        // Act
        var result = cls.Update(null, null, sameDate, sameDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(sameDate, result.Value.StartDate);
        Assert.Equal(sameDate, result.Value.EndDate);
    }

    [Fact]
    public void Update_ErrorOccurs_ShouldNotMutateOriginalObject()
    {
        // Arrange
        var cls = CreateValidClass();
        var originalName = cls.Name;
        var originalSemester = cls.Semester;
        var originalStartDate = cls.StartDate;
        var originalEndDate = cls.EndDate;

        // Act - try to update with invalid dates
        var result = cls.Update("New Name", "New Semester", _validEndDate, _validStartDate);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(originalName, cls.Name);
        Assert.Equal(originalSemester, cls.Semester);
        Assert.Equal(originalStartDate, cls.StartDate);
        Assert.Equal(originalEndDate, cls.EndDate);
    }
}