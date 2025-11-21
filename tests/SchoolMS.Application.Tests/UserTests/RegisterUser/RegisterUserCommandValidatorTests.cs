using SchoolMS.Application.Features.Identity.Commands.RegisterUser;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Tests.UserTests.RegisterUser;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_NoValidationErrors()
    {
        var command = new RegisterUserCommand
        {
            Name = "Alice Example",
            Email = "alice@example.test",
            Password = "Aa1!secure",
            Role = Role.Student
        };

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_EmptyName_ReturnsNameRequiredError()
    {
        var command = new RegisterUserCommand
        {
            Name = string.Empty,
            Email = "alice@example.test",
            Password = "Aa1!secure",
            Role = Role.Student
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
    }

    [Fact]
    public void Validate_NameTooLong_ReturnsMaximumLengthError()
    {
        var command = new RegisterUserCommand
        {
            Name = new string('A', 101),
            Email = "alice@example.test",
            Password = "Aa1!secure",
            Role = Role.Student
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage == "Name cannot exceed 100 characters.");
    }

    [Fact]
    public void Validate_InvalidEmail_ReturnsEmailAddressError()
    {
        var command = new RegisterUserCommand
        {
            Name = "Alice",
            Email = "not-an-email",
            Password = "Aa1!secure",
            Role = Role.Student
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email" && e.ErrorMessage == "Email address is not valid.");
    }

    [Fact]
    public void Validate_PasswordTooShort_ReturnsMinimumLengthError()
    {
        var command = new RegisterUserCommand
        {
            Name = "Alice",
            Email = "alice@example.test",
            Password = "Aa1!",
            Role = Role.Student
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password" && e.ErrorMessage == "Password must be at least 6 characters long.");
    }

    [Fact]
    public void Validate_PasswordMissingUppercase_ReturnsUppercaseRequirementError()
    {
        var command = new RegisterUserCommand
        {
            Name = "Alice",
            Email = "alice@example.test",
            Password = "aa1!lowercase",
            Role = Role.Student
        };

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Password" && e.ErrorMessage == "Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Validate_RoleAdmin_ReturnsRoleShouldBeStudentOrTeacher()
    {
        var command = new RegisterUserCommand
        {
            Name = "Alice",
            Email = "zerobyte@gmail.com",
            Password = "Aa1!secure",
            Role = Role.Admin
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Role" && e.ErrorMessage == "Role must be either student or teacher.");
    }
}