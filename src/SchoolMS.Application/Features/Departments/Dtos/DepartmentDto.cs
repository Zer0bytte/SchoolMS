namespace SchoolMS.Application.Features.Departments.Dtos;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public HeadOfDepartmentDto HeadOfDepartment { get; set; }
    public DateTimeOffset CreatedDateUtc { get; set; }
}
