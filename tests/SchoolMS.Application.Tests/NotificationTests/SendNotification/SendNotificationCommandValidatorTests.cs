using SchoolMS.Application.Features.Classes.Commands.CreateClass;
using SchoolMS.Application.Features.Notifications.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Tests.NotificationTests.SendNotification;

public class SendNotificationCommandValidatorTests
{

    [Fact]
    public void Validate_WithValidParams_ShouldReturnIsValid()
    {
        // Arrange
        var command = new SendNotificationCommand
        {
            IsClass = false,
            Title = "Test",
            Message = "Test",
            StudentId = Guid.NewGuid()
        };

        var validator = new SendNotificationCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenForStudentAndStudentIdInvalid_ShouldReturnError()
    {
        // Arrange
        var command = new SendNotificationCommand
        {
            IsClass = false,
            Title = "Test",
            Message = "Test",
        };

        var validator = new SendNotificationCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "StudentId");
    }

    [Fact]
    public void Validate_WhenForStudentAndStudentIdIsEmptyGuid_ShouldReturnError()
    {
        // Arrange
        var command = new SendNotificationCommand
        {
            IsClass = false,
            Title = "Test",
            Message = "Test",
            StudentId = Guid.Empty
        };

        var validator = new SendNotificationCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "StudentId");
    }


    [Fact]
    public void Validate_WhenEmptyTitle_ShouldReturnError()
    {
        // Arrange
        var command = new SendNotificationCommand
        {
            IsClass = false,
            Title = "",
            Message = "Test",
            StudentId = Guid.Empty
        };

        var validator = new SendNotificationCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }


    [Fact]
    public void Validate_WhenEmptyMessage_ShouldReturnError()
    {
        // Arrange
        var command = new SendNotificationCommand
        {
            IsClass = false,
            Title = "TTT",
            Message = "",
            StudentId = Guid.Empty
        };

        var validator = new SendNotificationCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "Message");
    }


    [Fact]
    public void Validate_WhenForClassAndClassIdInvalid_ShouldReturnError()
    {
        // Arrange
        var command = new SendNotificationCommand
        {
            IsClass = true,
            Title = "Title",
            Message = "Test",
            ClassId = Guid.Empty
        };

        var validator = new SendNotificationCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.Contains(result.Errors, e => e.PropertyName == "ClassId");
    }
}
