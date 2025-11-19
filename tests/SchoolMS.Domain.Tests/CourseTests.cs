using SchoolMS.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Domain.Tests;

public class CourseTests
{

    [Fact]
    public void CreateCourse_WithValidParameters_ShouldCreateCourse()
    {
        // Arrange
        var courseName = "Mathematics";
        var courseCode = "MATH101";
        var courseDescription = "bla bla bla";
        var credits = 3;
        // Act
        var course = Course.Create(Guid.NewGuid(), courseName, courseCode, courseDescription, Guid.NewGuid(), credits).Value;
        // Assert
        Assert.Equal(courseName, course.Name);
        Assert.Equal(courseCode, course.Code);
        Assert.Equal(credits, course.Credits);
    }


    [Fact]
    public void CreateCourse_WithEmptyName_ShouldReturnErrorNameRequired()
    {
        // Arrange
        var courseName = "";
        var courseCode = "MATH101";
        var courseDescription = "bla bla bla";
        var credits = -1;
        // Act
        var result = Course.Create(Guid.NewGuid(), courseName, courseCode, courseDescription, Guid.NewGuid(), credits);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.NameRequired, result.TopError);
    }


    [Fact]
    public void CreateCourse_WithEmptyDescription_ShouldReturnErrorDescriptionRequired()
    {
        // Arrange
        var courseName = "Mathematics";
        var courseCode = "MATH101";
        var courseDescription = "";
        var credits = 3;
        // Act
        var result = Course.Create(Guid.NewGuid(), courseName, courseCode, courseDescription, Guid.NewGuid(), credits);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.DescriptionRequired, result.TopError);
    }


    [Fact]
    public void CreateCourse_WithEmptyCode_ShouldReturnErrorCodeRequired()
    {
        // Arrange
        var courseName = "Mathematics";
        var courseCode = "";
        var courseDescription = "bla bla bla";
        var credits = 3;
        // Act
        var result = Course.Create(Guid.NewGuid(), courseName, courseCode, courseDescription, Guid.NewGuid(), credits);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.CodeRequired, result.TopError);
    }

    [Fact]
    public void CreateCourse_WithInvalidCredits_ShouldReturnErrorCreditsInvalid()
    {
        // Arrange
        var courseName = "Mathematics";
        var courseCode = "MATH101";
        var courseDescription = "bla bla bla";
        var credits = 0;
        // Act
        var result = Course.Create(Guid.NewGuid(), courseName, courseCode, courseDescription, Guid.NewGuid(), credits);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.CreditsInvalid, result.TopError);
    }

    [Fact]
    public void CreateCourse_WithEmptyDepartmentId_ShouldReturnErrorDepartmentRequired()
    {
        // Arrange
        var courseName = "Mathematics";
        var courseCode = "MATH101";
        var courseDescription = "bla bla bla";
        var credits = 3;
        // Act
        var result = Course.Create(Guid.NewGuid(), courseName, courseCode, courseDescription, Guid.Empty, credits);
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(CourseErrors.DepartmentRequired, result.TopError);
    }

   


 
}
