using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Departments.Dtos;

namespace SchoolMS.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsQuery : CursorQuery, IRequest<Result<DepartmentResult>>
{
    public string? DepartmentName { get; set; }
}
