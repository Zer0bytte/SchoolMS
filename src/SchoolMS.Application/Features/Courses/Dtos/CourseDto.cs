namespace SchoolMS.Application.Features.Courses.Dtos;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public Guid DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int Credits { get; set; }
    public DateTimeOffset CreatedDateUTC { get; set; }
}
