using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Classes.Queries.Student.GetStudentAttendance;
using SchoolMS.Application.Features.Classes.Queries.Student.GetStudentClasses;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.StudentClasses;

namespace SchoolMS.Application.Tests.ClassTests.Student.GetStudentClasses;

public class GetStudentClassesQueryHandlerTests
{
    public Mock<ILogger<GetStudentClassesQueryHandler>> Logger { get; set; } = new();

    [Fact]
    public async Task Hanlde_WhenStudentEnrolledToOneClass_ShouldReturnStudentEnrolledClassesOnly()
    {
        //Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var student = TestDbHelper.CreateStudent();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);
        var cls2 = TestDbHelper.CreateClass(course, teacher);
        context.StudentClasses.Add(new Domain.StudentClasses.StudentClass
        {
            ClassId = cls.Id,
            StudentId = student.Id,
            EnrollmentDate = DateTime.UtcNow,
        });
        context.Users.Add(teacher);
        context.Users.Add(student);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        context.Classes.Add(cls2);

        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var query = new GetStudentClassesQuery
        {
            Limit = 10,
        };

        var handler = new GetStudentClassesQueryHandler(context, user.Object,Logger.Object);

        //Act
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert

        Assert.Single(result!.Value.Items);
        Assert.Equal(result!.Value.Items.First().Id, cls.Id);
    }

    [Fact]
    public async Task Handle_WhenStudentEnrolledToMultipleClasses_ShouldReturnOnlyHisClasses()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        var student = TestDbHelper.CreateStudent();
        var dept = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(dept);

        var cls1 = TestDbHelper.CreateClass(course, teacher);
        var cls2 = TestDbHelper.CreateClass(course, teacher);
        var cls3 = TestDbHelper.CreateClass(course, teacher);

        context.StudentClasses.AddRange(
            new StudentClass { ClassId = cls1.Id, StudentId = student.Id, EnrollmentDate = DateTime.UtcNow },
            new StudentClass { ClassId = cls3.Id, StudentId = student.Id, EnrollmentDate = DateTime.UtcNow }
        );

        context.Users.AddRange(teacher, student);
        context.Departments.Add(dept);
        context.Courses.Add(course);
        context.Classes.AddRange(cls1, cls2, cls3);

        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var handler = new GetStudentClassesQueryHandler(context, user.Object, Logger.Object);

        // Act
        var result = await handler.Handle(new GetStudentClassesQuery { Limit = 10 }, CancellationToken.None);

        // Assert
        Assert.Equal(2, result!.Value.Items.Count());
        Assert.Contains(result.Value.Items, x => x.Id == cls1.Id);
        Assert.Contains(result.Value.Items, x => x.Id == cls3.Id);
    }
    [Fact]
    public async Task Handle_WhenCursorIsInvalid_ShouldReturnInvalidCursorError()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();

        var student = TestDbHelper.CreateStudent();
        context.Users.Add(student);
        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var handler = new GetStudentClassesQueryHandler(context, user.Object, Logger.Object);

        var query = new GetStudentClassesQuery
        {
            Limit = 10,
            Cursor = "INVALID_CURSOR_VALUE"
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("InvalidCursor", result.TopError.Code);
    }


    [Fact]
    public async Task Handle_WhenCursorIsValid_ShouldReturnPagedResults()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        var student = TestDbHelper.CreateStudent();
        var dept = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(dept);

        var cls1 = TestDbHelper.CreateClass(course, teacher);
        var cls2 = TestDbHelper.CreateClass(course, teacher);
        var cls3 = TestDbHelper.CreateClass(course, teacher);

        // Enroll student in all
        context.StudentClasses.AddRange(
            new StudentClass { ClassId = cls1.Id, StudentId = student.Id, EnrollmentDate = DateTime.UtcNow },
            new StudentClass { ClassId = cls2.Id, StudentId = student.Id, EnrollmentDate = DateTime.UtcNow },
            new StudentClass { ClassId = cls3.Id, StudentId = student.Id, EnrollmentDate = DateTime.UtcNow }
        );

        context.Users.AddRange(teacher, student);
        context.Departments.Add(dept);
        context.Courses.Add(course);
        context.Classes.AddRange(cls1, cls2, cls3);

        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var handler = new GetStudentClassesQueryHandler(context, user.Object, Logger.Object);

        // Step 1 — first page
        var firstPage = await handler.Handle(new GetStudentClassesQuery { Limit = 2 }, CancellationToken.None);

        Assert.Equal(2, firstPage.Value.Items.Count());
        Assert.True(firstPage.Value.HasMore);
        Assert.NotNull(firstPage.Value.Cursor);

        // Step 2 — second page using cursor
        var secondPage = await handler.Handle(new GetStudentClassesQuery
        {
            Limit = 2,
            Cursor = firstPage.Value.Cursor
        }, CancellationToken.None);

        // Assert
        Assert.Single(secondPage.Value.Items);
        Assert.False(secondPage.Value.HasMore);
    }

    [Fact]
    public async Task Handle_WhenStudentHasNoClasses_ShouldReturnEmptyList()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();

        var student = TestDbHelper.CreateStudent();
        context.Users.Add(student);
        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var handler = new GetStudentClassesQueryHandler(context, user.Object, Logger.Object);

        // Act
        var result = await handler.Handle(new GetStudentClassesQuery { Limit = 10 }, CancellationToken.None);

        // Assert
        Assert.Empty(result.Value.Items);
        Assert.False(result.Value.HasMore);
        Assert.Null(result.Value.Cursor);
    }


    [Fact]
    public async Task Handle_WhenLimitSmallerThanReturned_ShouldReturnOnlyLimitItems()
    {
        // Arrange
        var user = new Mock<IUser>();
        using var context = TestDbHelper.CreateContext();

        var teacher = TestDbHelper.CreateTeacher();
        var student = TestDbHelper.CreateStudent();
        var dept = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(dept);

        var cls1 = TestDbHelper.CreateClass(course, teacher);
        var cls2 = TestDbHelper.CreateClass(course, teacher);
        var cls3 = TestDbHelper.CreateClass(course, teacher);

        context.StudentClasses.AddRange(
            new StudentClass { ClassId = cls1.Id, StudentId = student.Id, EnrollmentDate = DateTime.UtcNow },
            new StudentClass { ClassId = cls2.Id, StudentId = student.Id, EnrollmentDate = DateTime.UtcNow },
            new StudentClass { ClassId = cls3.Id, StudentId = student.Id, EnrollmentDate = DateTime.UtcNow }
        );

        context.Users.AddRange(teacher, student);
        context.Departments.Add(dept);
        context.Courses.Add(course);
        context.Classes.AddRange(cls1, cls2, cls3);

        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(student.Id.ToString());

        var handler = new GetStudentClassesQueryHandler(context, user.Object, Logger.Object);

        // Act
        var result = await handler.Handle(new GetStudentClassesQuery { Limit = 1 }, CancellationToken.None);

        // Assert
        Assert.Single(result.Value.Items);
        Assert.True(result.Value.HasMore);
        Assert.NotNull(result.Value.Cursor);
    }

}
