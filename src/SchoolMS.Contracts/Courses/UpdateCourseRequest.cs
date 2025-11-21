namespace SchoolMS.Contracts.Courses;

public class UpdateCourseRequest
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid? DepartmentId { get; set; }
    public int Credits { get; set; }
}
