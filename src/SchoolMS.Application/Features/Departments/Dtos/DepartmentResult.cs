namespace SchoolMS.Application.Features.Departments.Dtos;

public class DepartmentResult
{
    public List<DepartmentDto> Items { get; set; } = [];
    public string? Cursor { get; set; }
    public bool HasMore { get; set; }
}
