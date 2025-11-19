using SchoolMS.Domain.Departments;

namespace SchoolMS.Domain.Tests;

public class DepartmentTests
{
    private const string ValidName = "Computer Science";
    private const string ValidDescription = "Computer science department is a bla bla bla";



    [Fact]
    public async Task Create_WithValidData_ReturnsSuccessAndInitializesProperties()
    {
        // Act
        var result = Department.Create(Guid.CreateVersion7(), ValidName, ValidDescription, Guid.NewGuid());
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        var department = result.Value;
        Assert.Equal(ValidName, department.Name);
        Assert.Equal(ValidDescription, department.Description);
        Assert.Empty(department.Courses);

    }

    [Fact]
    public async Task Create_WithEmptyName_ReturnsNameRequiredError()
    {
        // Act
        var result = Department.Create(Guid.CreateVersion7(), string.Empty, ValidDescription, Guid.NewGuid());
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DepartmentErrors.NameRequired, result.TopError);
    }

    [Fact]
    public async Task Create_WithEmptyDescription_ReturnsDescriptionRequiredError()
    {
        // Act
        var result = Department.Create(Guid.CreateVersion7(), ValidName, string.Empty, Guid.NewGuid());
        // Assert
        Assert.True(result.IsError);
        Assert.Equal(DepartmentErrors.DescriptionRequired, result.TopError);
    }

}
