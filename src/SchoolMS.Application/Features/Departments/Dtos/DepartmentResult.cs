using SchoolMS.Application.Common.Models;

namespace SchoolMS.Application.Features.Departments.Dtos;

public class DepartmentResult : CurosrResult
{
    public List<DepartmentDto> Items { get; set; } = [];
}
