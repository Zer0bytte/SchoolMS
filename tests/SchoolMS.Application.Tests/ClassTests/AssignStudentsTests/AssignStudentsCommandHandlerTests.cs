using Microsoft.EntityFrameworkCore;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Classes.Commands.AssignStudents;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Tests.ClassTests.AssignStudentsTests;

public class AssignStudentsCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidStudentIds_ShouldAddStudentsToClass()
    {
        //Arrange
        var user = new Mock<IUser>();

        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);
        var student1 = TestDbHelper.CreateStudent();
        var student2 = TestDbHelper.CreateStudent();
        context.Users.Add(teacher);
        context.Users.Add(student1);
        context.Users.Add(student2);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var command = new AssignStudentsCommand
        {
            ClassId = cls.Id,
            StudentIds = [student1.Id, student2.Id],
        };

        var handler = new AssignStudentsCommandHandler(context, user.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        var dbClass = await context.Classes.FirstOrDefaultAsync(c => c.Id == cls.Id);

        Assert.Equal(2, dbClass.StudentClasses.Count);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }


    [Fact]
    public async Task Handle_WithOnlyOneValidStudentId_ShouldReturnError()
    {
        //Arrange
        var user = new Mock<IUser>();

        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher);
        var student1 = TestDbHelper.CreateStudent();
        context.Users.Add(teacher);
        context.Users.Add(student1);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        await context.SaveChangesAsync();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var command = new AssignStudentsCommand
        {
            ClassId = cls.Id,
            StudentIds = [student1.Id, Guid.NewGuid()],
        };

        var handler = new AssignStudentsCommandHandler(context, user.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsError);

        Assert.Equal("Students.NotFound", result.TopError.Code);
    }


    [Fact]
    public async Task Handle_ClassWithAnotherTeacher_ShouldReturnClassNotFound()
    {
        var user = new Mock<IUser>();

        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var teacher2 = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher2);
        var course = TestDbHelper.CreateCourse(department);
        var cls = TestDbHelper.CreateClass(course, teacher2);
        var student1 = TestDbHelper.CreateStudent();
        context.Users.Add(teacher);
        context.Users.Add(teacher2);
        context.Users.Add(student1);
        context.Departments.Add(department);
        context.Courses.Add(course);
        context.Classes.Add(cls);
        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var command = new AssignStudentsCommand
        {
            ClassId = cls.Id,
            StudentIds = [student1.Id, Guid.NewGuid()],
        };

        var handler = new AssignStudentsCommandHandler(context, user.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsError);

        Assert.Equal(ClassErrors.NotFound, result.TopError);
    }



    [Fact]
    public async Task Handle_InvalidClassId_ShouldReturnClassNotFound()
    {
        var user = new Mock<IUser>();

        using var context = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var student1 = TestDbHelper.CreateStudent();
        context.Users.Add(teacher);
        context.Users.Add(student1);
        await context.SaveChangesAsync();

        user.Setup(u => u.Id).Returns(teacher.Id.ToString());

        var command = new AssignStudentsCommand
        {
            ClassId = Guid.NewGuid(),
            StudentIds = [student1.Id, Guid.NewGuid()],
        };

        var handler = new AssignStudentsCommandHandler(context, user.Object);

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.NotNull(result);
        Assert.True(result.IsError);

        Assert.Equal(ClassErrors.NotFound, result.TopError);
    }

}
