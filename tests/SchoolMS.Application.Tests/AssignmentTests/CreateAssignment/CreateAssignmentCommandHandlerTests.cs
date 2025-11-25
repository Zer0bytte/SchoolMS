using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Tests.AssignmentTests.CreateAssignment;

public class CreateAssignmentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidParms_ShouldCreateAssignment()
    {
        var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);

        context.Users.Add(teacher);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        await context.SaveChangesAsync();
        var user = new Mock<IUser>();
        var logger = new Mock<ILogger<CreateAssignmentCommandHandler>>();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var command = new CreateAssignmentCommand
        {
            ClassId = cls.Id,
            Description = "Desc",
            DueDate = new DateOnly(2025, 12, 12),
            Title = "Title"
        };

        var handler = new CreateAssignmentCommandHandler(context, user.Object, logger.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(command.Title, result.Value.Title);
        Assert.Equal(command.Description, result.Value.Description);
        Assert.Equal(command.DueDate, result.Value.DueDate);
        Assert.True(result.Value.Id != Guid.Empty);


        var assignmentCount = await context.Assignments.CountAsync();
        Assert.Equal(1, assignmentCount);
    }



    [Fact]
    public async Task Handle_WithInvalidClassId_ShouldReturnClassNotFound()
    {
        var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        await context.SaveChangesAsync();
        var user = new Mock<IUser>();
        var logger = new Mock<ILogger<CreateAssignmentCommandHandler>>();

        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var command = new CreateAssignmentCommand
        {
            ClassId = Guid.NewGuid(),
            Description = "Desc",
            DueDate = new DateOnly(2025, 12, 12),
            Title = "Title"
        };

        var handler = new CreateAssignmentCommandHandler(context, user.Object, logger.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsError);
        Assert.Equal(ClassErrors.NotFound, result.TopError);
    }

}
