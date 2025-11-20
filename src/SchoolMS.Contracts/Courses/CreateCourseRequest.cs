namespace SchoolMS.Contracts.Courses;

public class CreateCourseRequest
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public int Credits { get; set; }
}
