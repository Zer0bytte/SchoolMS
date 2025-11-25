using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;
using SchoolMS.Application.Features.Classes.Queries.Teacher.GetClassAttendance;
using SchoolMS.Application.Features.Courses.Commands.CreateCourse;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Tests.CoursTests.CreateCourseTests;

public class CreateCourseCommandHandlerTests
{
    public Mock<ILogger<CreateCourseCommandHandler>> Logger { get; set; } = new();
    public Mock<HybridCache> Cache { get; set; } = new();

    [Fact]
    public async Task Handle_GivenValidRequest_ShouldCreateCourse()
    {
        //Arrange
        using var dbContext = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        dbContext.Users.Add(teacher);
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync();
        var user = new Mock<IUser>();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var handler = new CreateCourseCommandHandler(dbContext, user.Object, Logger.Object, Cache.Object);
        var command = new CreateCourseCommand
        {
            Name = "Introduction to Programming",
            Description = "A beginner course on programming concepts.",
            DepartmentId = department.Id,
            Code = "CS101",
            Credits = 3
        };

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(command.Description, result.Value.Description);
        Assert.Equal(command.DepartmentId, result.Value.DepartmentId);
        Assert.Equal(command.Code, result.Value.Code);
        Assert.Equal(command.Credits, result.Value.Credits);


        var dbcourse = dbContext.Courses.SingleOrDefault(c => c.Id == result.Value.Id);
        Assert.Equal(dbcourse.Name, result.Value.Name);
        Assert.Equal(dbcourse.Description, result.Value.Description);
        Assert.Equal(dbcourse.DepartmentId, result.Value.DepartmentId);
        Assert.Equal(dbcourse.Code, result.Value.Code);
        Assert.Equal(dbcourse.Credits, result.Value.Credits);
    }


    [Fact]
    public async Task Handle_GivenDuplicateCourseCode_ShouldReturnError()
    {
        //Arrange
        using var dbContext = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        dbContext.Users.Add(teacher);
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync();
        var existingCourse = TestDbHelper.CreateCourse(department);
        dbContext.Courses.Add(existingCourse);
        await dbContext.SaveChangesAsync();
        var user = new Mock<IUser>();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var handler = new CreateCourseCommandHandler(dbContext, user.Object, Logger.Object, Cache.Object);
        var command = new CreateCourseCommand
        {
            Name = "Advanced Programming",
            Description = "An advanced course on programming concepts.",
            DepartmentId = department.Id,
            Code = existingCourse.Code,
            Credits = 4
        };
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e == CourseErrors.DuplicateCode);
    }


    [Fact]
    public async Task Handle_GivenDuplicateCourseName_ShouldReturnError()
    {
        //Arrange
        using var dbContext = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        dbContext.Users.Add(teacher);
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync();
        var existingCourse = TestDbHelper.CreateCourse(department);
        dbContext.Courses.Add(existingCourse);
        await dbContext.SaveChangesAsync();
        var user = new Mock<IUser>();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var handler = new CreateCourseCommandHandler(dbContext, user.Object, Logger.Object, Cache.Object);
        var command = new CreateCourseCommand
        {
            Name = existingCourse.Name,
            Description = "An advanced course on programming concepts.",
            DepartmentId = department.Id,
            Code = "CS201",
            Credits = 4
        };
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e == CourseErrors.DuplicateName);
    }


    [Fact]
    public async Task Handle_GivenInvalidDepartmentId_ShouldReturnError()
    {
        //Arrange
        using var dbContext = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        dbContext.Users.Add(teacher);
        await dbContext.SaveChangesAsync();
        var user = new Mock<IUser>();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var handler = new CreateCourseCommandHandler(dbContext, user.Object, Logger.Object, Cache.Object);
        var command = new CreateCourseCommand
        {
            Name = "Data Structures",
            Description = "A course on data structures.",
            DepartmentId = Guid.NewGuid(), // Invalid DepartmentId
            Code = "CS102",
            Credits = 3
        };
        //Act
        var result = await handler.Handle(command, CancellationToken.None);
        //Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e == DepartmentErrors.NotFound);
    }

    [Fact]
    public async Task Handle_GivenDuplicateCourseCodeDifferentDepartment_ShouldCreateCourse()
    {
        //Arrange
        using var dbContext = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);

        var anotherDepartment = TestDbHelper.CreateDepartment(teacher, "Mathematics");

        dbContext.Users.Add(teacher);
        dbContext.Departments.Add(department);
        dbContext.Departments.Add(anotherDepartment);
        await dbContext.SaveChangesAsync();

        var existingCourse = TestDbHelper.CreateCourse(department);
        dbContext.Courses.Add(existingCourse);
        await dbContext.SaveChangesAsync();
        var user = new Mock<IUser>();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var handler = new CreateCourseCommandHandler(dbContext, user.Object, Logger.Object, Cache.Object);

        var command = new CreateCourseCommand
        {
            Name = "Calculus I",
            Description = "An introductory course on calculus.",
            DepartmentId = anotherDepartment.Id,
            Code = existingCourse.Code,
            Credits = 4
        };


        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_GivenDuplicateCourseNameDifferentDepartment_ShouldCreateCourse()
    {
        //Arrange
        using var dbContext = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);

        var anotherDepartment = TestDbHelper.CreateDepartment(teacher, "Mathematics");
        dbContext.Users.Add(teacher);
        dbContext.Departments.Add(department);
        dbContext.Departments.Add(anotherDepartment);
        await dbContext.SaveChangesAsync();
        var existingCourse = TestDbHelper.CreateCourse(department);
        dbContext.Courses.Add(existingCourse);
        await dbContext.SaveChangesAsync();
        var user = new Mock<IUser>();
        user.Setup(u => u.Id).Returns(teacher.Id.ToString());
        var handler = new CreateCourseCommandHandler(dbContext, user.Object, Logger.Object, Cache.Object);
        var command = new CreateCourseCommand
        {
            Name = existingCourse.Name,
            Description = "An introductory course on calculus.",
            DepartmentId = anotherDepartment.Id,
            Code = "MATH101",
            Credits = 4
        };

        //Act
        var result = await handler.Handle(command, CancellationToken.None);

        //Assert

        Assert.True(result.IsSuccess);
    }
}
