
using SchoolMS.Domain.Classes;

namespace SchoolMS.Domain.Tests.Classes;

public class ClassUpdateTests
{
    [Fact]
    public void Update_ValidParams_ShouldUpdate()
    {
        //Arrange
        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 20);
        var cls = Class.Create(Guid.NewGuid(), "name", Guid.NewGuid(), Guid.NewGuid(), "sem", start, end).Value;

        //Act
        var result = cls.Update("newname", "newsem", start, end);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Update_EmptyName_ShouldNotChangeName()
    {
        //Arrange
        //Arrange
        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 20);
        var cls = Class.Create(Guid.NewGuid(), "name", Guid.NewGuid(), Guid.NewGuid(), "sem", start, end).Value;

        //Act
        var result = cls.Update("", "newsem", start, end);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, cls.Name);
    }

    [Fact]
    public void Update_EmptySemester_ShouldNotChangeSemester()
    {
        //Arrange
        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 20);
        var cls = Class.Create(Guid.NewGuid(), "name", Guid.NewGuid(), Guid.NewGuid(), "sem", start, end).Value;

        //Act
        var result = cls.Update(cls.Name, "", start, end);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, cls.Name);
        Assert.Equal(result.Value.StartDate, cls.StartDate);
        Assert.Equal(result.Value.EndDate, cls.EndDate);
        Assert.Equal(result.Value.Semester, cls.Semester);

    }


    [Fact]
    public void Update_WithDefaultStartDate_ShouldNotChangeStartDate()
    {
        //Arrange
        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 20);
        var cls = Class.Create(Guid.NewGuid(), "name", Guid.NewGuid(), Guid.NewGuid(), "sem", start, end).Value;

        //Act
        var result = cls.Update(cls.Name, "", default, end);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Name, cls.Name);
        Assert.Equal(result.Value.StartDate, cls.StartDate);
        Assert.Equal(result.Value.EndDate, cls.EndDate);
        Assert.Equal(result.Value.Semester, cls.Semester);
    }


    [Fact]
    public void Update_WithDefaultEndDate_ShouldReturnEndDateBeforeStartDate()
    {
        //Arrange
        var start = new DateOnly(2025, 1, 10);
        var end = new DateOnly(2025, 1, 20);
        var cls = Class.Create(Guid.NewGuid(), "name", Guid.NewGuid(), Guid.NewGuid(), "sem", start, end).Value;

        //Act
        var result = cls.Update(cls.Name, "", start, default);

        //Assert
        Assert.True(result.IsError);
        Assert.Equal(result.TopError, ClassErrors.EndDateBeforeStartDate);

    }

}
