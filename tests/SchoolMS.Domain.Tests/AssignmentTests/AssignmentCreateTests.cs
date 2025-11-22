using SchoolMS.Domain.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Domain.Tests.AssignmentTests;

public class AssignmentCreateTests
{
    private readonly Guid _classId = Guid.NewGuid();
    private readonly Guid _teacherId = Guid.NewGuid();
    private readonly Guid _assignmentId = Guid.NewGuid();
    private readonly DateOnly _today = new DateOnly(2025, 11, 22);

    [Fact]
    public void Create_ShouldSucceed_WhenAllFieldsAreValid_AndDueDateInFuture()
    {
        // Arrange
        var dueDate = _today.AddDays(1);

        // Act
        var result = Assignment.Create(
            _assignmentId,
            _classId,
            "Math Homework",
            "Solve problems",
            dueDate,
            _today,
            _teacherId,
            _today
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Math Homework", result.Value.Title);
        Assert.Equal(dueDate, result.Value.DueDate);
    }

    [Fact]
    public void Create_ShouldFail_WhenTitleIsEmpty()
    {
        var dueDate = _today.AddDays(1);

        var result = Assignment.Create(
            _assignmentId,
            _classId,
            "",
            "Description",
            dueDate,
            _today,
            _teacherId,
            _today
        );

        Assert.True(result.IsError);
        Assert.Equal(AssignmentErrors.TitleRequired, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WhenDescriptionIsEmpty()
    {
        var dueDate = _today.AddDays(1);

        var result = Assignment.Create(
            _assignmentId,
            _classId,
            "Title",
            "",                 // invalid description
            dueDate,
            _today,
            _teacherId,
            _today
        );

        Assert.True(result.IsError);
        Assert.Equal(AssignmentErrors.DescriptionRequired, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WhenDueDateIsToday()
    {
        var dueDate = _today;  // due date = today → invalid

        var result = Assignment.Create(
            _assignmentId,
            _classId,
            "Title",
            "Description",
            dueDate,
            _today,
            _teacherId,
            _today
        );

        Assert.True(result.IsError);
        Assert.Equal(AssignmentErrors.DueDateMustBeInFuture, result.TopError);
    }

    [Fact]
    public void Create_ShouldFail_WhenDueDateIsInPast()
    {
        var dueDate = _today.AddDays(-1);

        var result = Assignment.Create(
            _assignmentId,
            _classId,
            "Title",
            "Description",
            dueDate,
            _today,
            _teacherId,
            _today
        );

        Assert.True(result.IsError);
        Assert.Equal(AssignmentErrors.DueDateMustBeInFuture, result.TopError);
    }
}
