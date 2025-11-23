using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Courses.Dtos;

namespace SchoolMS.Application.Features.Courses.Queries.GetCourses;

public class GetCoursesQuery : CursorQuery, IRequest<Result<CursorResult<CourseDto>>>
{
    public Guid? DepartmentId { get; set; }
    public string? SearchTerm { get; set; }

}
