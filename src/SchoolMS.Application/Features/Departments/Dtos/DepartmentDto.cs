namespace SchoolMS.Application.Features.Departments.Dtos;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? HeadOfDepartmentName { get; set; }
    public Guid? HeadOfDepartmentId { get; set; }
    public DateTimeOffset CreatedDateUtc { get; set; }
}
