using SchoolMS.Application.Features.Departments.Queries.GetDepartment;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Tests.Departments.GetDepartmentByIdTests;

public class GetDepartmentByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnDepartment_WhenDepartmentExists()
    {
        //Arrage
        using var dbContext = TestDbHelper.CreateContext();
        var teacher = TestDbHelper.CreateTeacher();
        var department = TestDbHelper.CreateDepartment(teacher);
        dbContext.Users.Add(teacher);
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync();
        //Act
        var handler = new GetDepartmentByIdQueryHandler(dbContext);
        var query = new GetDepartmentByIdQuery
        {
            Id = department.Id
        };

        var result = await handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(department.Name, result.Value.Name);
        Assert.Equal(department.Description, result.Value.Description);
        Assert.Equal(department.HeadOfDepartmentId, result.Value.HeadOfDepartmentId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenDepartmentDoesNotExist()
    {
        //Arrage
        using var dbContext = TestDbHelper.CreateContext();
        //Act
        var handler = new GetDepartmentByIdQueryHandler(dbContext);
        var query = new GetDepartmentByIdQuery
        {
            Id = Guid.NewGuid()
        };
        var result = await handler.Handle(query, CancellationToken.None);
        //Assert
        Assert.NotNull(result);
        Assert.True(result.IsError);
        Assert.Equal(DepartmentErrors.NotFound.Code, result.TopError.Code);
    }
}
