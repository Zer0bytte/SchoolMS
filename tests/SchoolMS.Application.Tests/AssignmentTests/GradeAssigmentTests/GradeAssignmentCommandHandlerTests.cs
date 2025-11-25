using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Assignments.Commands.GradeAssignement;
using SchoolMS.Application.Features.Classes.Commands.CreateClass;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Submissions;
using SchoolMS.Domain.Users;

namespace SchoolMS.Application.Tests.AssignmentTests.GradeAssigmentTests;

public class GradeAssignmentCommandHandlerTests
{
    public Mock<ILogger<GradeAssignmentCommandHandler>> Logger { get; set; } = new();

    public TestAppDbContext Context { get; set; }
    public Mock<IUser> User { get; set; } = new Mock<IUser>();
    public GradeAssignmentCommandHandlerTests()
    {
        Context = TestDbHelper.CreateContext();

    }
    public async Task<Assignment> CreateAssignemnt()
    {
        User teacher = TestDbHelper.CreateTeacher();
        Department department = TestDbHelper.CreateDepartment(teacher);
        Course course = TestDbHelper.CreateCourse(department);
        Class cls = TestDbHelper.CreateClass(course, teacher);

        Assignment assignment = TestDbHelper.CreateAssignment(cls, teacher);
        Context.Assignments.Add(assignment);
        Context.Users.Add(teacher);
        Context.Departments.Add(department);
        Context.Courses.Add(course);
        Context.Classes.Add(cls);
        await Context.SaveChangesAsync();

        return assignment;
    }


    [Fact]
    public async Task Handle_WithValidParams_ShouldMarkAssigment()
    {
        //Arrange
        Assignment assigment = await CreateAssignemnt();
        User student = TestDbHelper.CreateStudent();
        Context.Users.Add(student);
        Context.StudentClasses.Add(new Domain.StudentClasses.StudentClass
        {
            StudentId = student.Id,
            ClassId = assigment.ClassId,
            EnrollmentDate = DateTime.Now,
        });
        Submission submission = Submission.Create(Guid.NewGuid(), assigment.Id, student.Id, DateTime.UtcNow, "http://www.google.com/file.pdf").Value;

        Context.Submissions.Add(submission);
        await Context.SaveChangesAsync();
        User.Setup(u => u.Id).Returns(assigment.CreatedByTeacherId.ToString());
        GradeAssignmentCommand command = new GradeAssignmentCommand
        {
            SubmissionId = submission.Id,
            Grade = 10,
            Remarks = "Remark SS"
        };

        GradeAssignmentCommandHandler handler = new GradeAssignmentCommandHandler(Context, User.Object, Logger.Object);
        //Act

        Result<Success> result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        Submission? sb = await Context.Submissions.FirstOrDefaultAsync(s => s.Id == submission.Id);
        Assert.Equal(command.Grade, sb.Grade);
        Assert.Equal(command.Remarks, sb.Remarks);

    }


    [Fact]
    public async Task Handle_WithInvalidParams_ShouldReturnNotFound()
    {
        //Arrange
        Assignment assigment = await CreateAssignemnt();

        User student = TestDbHelper.CreateStudent();
        Context.Users.Add(student);
        Context.StudentClasses.Add(new StudentClass
        {
            StudentId = student.Id,
            ClassId = assigment.ClassId,
            EnrollmentDate = DateTime.Now,
        });
        Submission submission = Submission.Create(Guid.NewGuid(), assigment.Id, student.Id, DateTime.UtcNow, "http://www.google.com/file.pdf").Value;

        Context.Submissions.Add(submission);

        var teacher2 = TestDbHelper.CreateTeacher();
        Context.Users.Add(teacher2);
        await Context.SaveChangesAsync();
        User.Setup(u => u.Id).Returns(teacher2.Id.ToString());
        GradeAssignmentCommand command = new GradeAssignmentCommand
        {
            SubmissionId = submission.Id,
            Grade = 10,
            Remarks = "Remark SS"
        };

        GradeAssignmentCommandHandler handler = new GradeAssignmentCommandHandler(Context, User.Object, Logger.Object);
        //Act

        Result<Success> result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsError);
        Assert.Equal(SubmissionErrors.NotFound, result.TopError);
    }
}
