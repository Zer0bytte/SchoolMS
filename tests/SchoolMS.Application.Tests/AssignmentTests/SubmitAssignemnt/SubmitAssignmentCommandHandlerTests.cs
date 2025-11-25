using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Assignments.Commands.SubmitAssignment;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Assignments;

namespace SchoolMS.Application.Tests.AssignmentTests.SubmitAssignemnt;

public class SubmitAssignmentCommandHandlerTests
{

    public TestAppDbContext Context { get; set; } = TestDbHelper.CreateContext();
    public Mock<IUser> User { get; set; } = new Mock<IUser>();
    public Mock<ILogger<SubmitAssignmentCommandHandler>> Logger { get; set; } = new();

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
    public async Task Handle_WithValidParams_ShouldCreateSubmission()
    {
        var assignemnt = await CreateAssignemnts();
        var student = TestDbHelper.CreateStudent();
        Context.StudentClasses.Add(new Domain.StudentClasses.StudentClass
        {
            ClassId = assignemnt.ClassId,
            StudentId = student.Id,
            EnrollmentDate = DateTime.Now,
        });

        await Context.SaveChangesAsync();

        var command = new SubmitAssignmentCommand
        {
            AssignmentId = assignemnt.Id,
            File = new FileData
            {
                FileName = "test.pdf",
                Content = new MemoryStream(new byte[] { 1, 2, 3 })
            }
        };
        User.Setup(u => u.Id).Returns(student.Id.ToString());
        var fs = new Mock<IFileStorageService>();
        fs.Setup(f => f.SaveFileAsync(command.File, "submissions")).ReturnsAsync("http://www.url.com/uploads/submissions/test.pdf");

        var handler = new SubmitAssignmentCommandHandler(Context, User.Object, fs.Object, Logger.Object);

        var result = await handler.Handle(command, CancellationToken.None);

    }

}
