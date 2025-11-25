using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;
using SchoolMS.Application.Features.Assignments.Qureies.GetAssignments;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Tests.AssignmentTests.GetAssignments;

public class GetAssignmentsQueryHandlerTests
{
    public TestAppDbContext Context { get; set; }
    public Mock<IUser> User { get; set; } = new Mock<IUser>();
    public Mock<ILogger<GetAssignmentsQueryHandler>> logger = new Mock<ILogger<GetAssignmentsQueryHandler>>();

    public GetAssignmentsQueryHandlerTests()
    {
        Context = TestDbHelper.CreateContext();

    }
    public async Task<Assignment> CreateAssignemnts(int count = 1)
    {
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);
        Assignment lastAdded = null!;
        for (int i = 0; i < count; i++)
        {
            lastAdded = TestDbHelper.CreateAssignment(cls, teacher);
            Context.Assignments.Add(lastAdded);

        }
        Context.Users.Add(teacher);
        Context.Departments.Add(department);
        Context.Courses.Add(course);
        Context.Classes.Add(cls);
        await Context.SaveChangesAsync();

        return lastAdded;
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenClassDoesNotExist()
    {
        //Arrange
        var query = new GetAssignmentsQuery
        {
            ClassId = Guid.NewGuid(),
        };

        User.Setup(u => u.Id).Returns(Guid.NewGuid().ToString());

        var handler = new GetAssignmentsQueryHandler(Context, User.Object, logger.Object);

        //Act
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.Equal(ClassErrors.NotFound, result.TopError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenCursorIsInvalid()
    {
        var assignment = await CreateAssignemnts();

        var query = new GetAssignmentsQuery
        {
            ClassId = assignment.ClassId,
            Cursor = "invalid-cursor"
        };

        User.Setup(u => u.Id).Returns(assignment.Class.TeacherId.ToString());

        var handler = new GetAssignmentsQueryHandler(Context, User.Object, logger.Object);

        //Act
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.Equal("InvalidCursor", result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnAssignments_WhenNoCursor()
    {
        var assignment = await CreateAssignemnts(5);

        var query = new GetAssignmentsQuery
        {
            ClassId = assignment.ClassId,
            Limit = 10
        };

        User.Setup(u => u.Id).Returns(assignment.Class.TeacherId.ToString());

        var handler = new GetAssignmentsQueryHandler(Context, User.Object, logger.Object);

        //Act
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.Equal(5, result.Value.Items.Count);
        Assert.Null(result.Value.Cursor);

    }

    [Fact]
    public async Task Handle_ShouldReturnNextCursor_WhenMoreItemsExist()
    {
        var assignment = await CreateAssignemnts(10);

        var query = new GetAssignmentsQuery
        {
            ClassId = assignment.ClassId,
            Limit = 3
        };

        User.Setup(u => u.Id).Returns(assignment.Class.TeacherId.ToString());

        var handler = new GetAssignmentsQueryHandler(Context, User.Object, logger.Object);

        //Act
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.Equal(3, result.Value.Items.Count);
        Assert.True(result.Value.HasMore);
        Assert.NotNull(result.Value.Cursor);
    }


    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenClassBelongsToAnotherTeacher()
    {
        //Arrange
        var assignment1 = await CreateAssignemnts();
        var assignment2 = await CreateAssignemnts();

        var query = new GetAssignmentsQuery
        {
            ClassId = assignment2.ClassId,
        };

        User.Setup(u => u.Id).Returns(assignment1.Class.TeacherId.ToString());

        var handler = new GetAssignmentsQueryHandler(Context, User.Object, logger.Object);

        //Act
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.Equal(ClassErrors.NotFound, result.TopError);
    }
}
