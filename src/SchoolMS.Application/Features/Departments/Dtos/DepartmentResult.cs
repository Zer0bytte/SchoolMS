using SchoolMS.Application.Common.Models;

namespace SchoolMS.Application.Features.Departments.Dtos;

public class DepartmentsResult : CurosrResult
{
    public List<DepartmentDto> Items { get; set; } = [];
}
