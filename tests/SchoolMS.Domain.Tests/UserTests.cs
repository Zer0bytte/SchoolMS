using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Domain.Tests;

public class UserTests
{
    private const string ValidName = "John Doe";
    private const string ValidEmail = "john.doe@school.edu";
    private const string ValidPassword = "SecurePassword123";
    private const Role ValidRole = Role.Student;

    [Fact]
    public void Create_WithValidData_ReturnsSuccessAndInitializesProperties()
    {
        // Arrange
        var expectedRole = Role.Teacher;

        // Act
        var result = User.Create(Guid.CreateVersion7(), ValidName, ValidEmail, ValidPassword, expectedRole);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var user = result.Value;
        Assert.Equal(ValidName, user.Name);
        Assert.Equal(ValidEmail, user.Email);
        Assert.Equal(ValidPassword, user.Password);
        Assert.Equal(expectedRole, user.Role);
        Assert.True(user.IsActive);
        Assert.Empty(user.ManagedDepartments);
        Assert.Empty(user.TaughtClasses);

        Assert.NotNull(user.StudentClasses);
        Assert.NotNull(user.Attendances);
        Assert.NotNull(user.Submissions);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Create_WithInvalidName_ReturnsNameRequiredError(string invalidName)
    {
        // Act
        var result = User.Create(Guid.CreateVersion7(), invalidName, ValidEmail, ValidPassword, ValidRole);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(UserErrors.NameRequired, result.Errors.First());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidEmail_ReturnsEmailRequiredError(string invalidEmail)
    {
        // Act
        var result = User.Create(Guid.CreateVersion7(), ValidName, invalidEmail, ValidPassword, ValidRole);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(UserErrors.EmailRequired, result.Errors.First());
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("invalid@")]
    [InlineData("missingdomain.")]
    [InlineData("space in@email.com")]
    public void Create_WithPoorlyFormattedEmail_ReturnsInvalidEmailError(string poorlyFormattedEmail)
    {
        // Act
        var result = User.Create(Guid.CreateVersion7(), ValidName, poorlyFormattedEmail, ValidPassword, ValidRole);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(UserErrors.InvalidEmail, result.Errors.First());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidPassword_ReturnsPasswordRequiredError(string invalidPassword)
    {
        // Act
        var result = User.Create(Guid.CreateVersion7(), ValidName, ValidEmail, invalidPassword, ValidRole);

        // Assert
        Assert.True(result.IsError);

        Assert.Equal(UserErrors.Password, result.Errors.First());
    }
}