using SchoolMS.Application.Common.Models;

namespace SchoolMS.Contracts.Departments;

public class GetDepartmentsRequest : CursorQuery
{
    public string? DepartmentName { get; set; }

}
