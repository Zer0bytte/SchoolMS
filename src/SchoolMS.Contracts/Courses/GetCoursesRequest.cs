using SchoolMS.Application.Common.Models;

namespace SchoolMS.Contracts.Courses;

public class GetCoursesRequest : CursorQuery
{
    public Guid? DepartmentId { get; set; }
    public string? SearchTerm { get; set; }
}
