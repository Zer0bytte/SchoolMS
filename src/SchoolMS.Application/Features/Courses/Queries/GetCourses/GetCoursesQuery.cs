using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Courses.Dtos;

namespace SchoolMS.Application.Features.Courses.Queries.GetCourses;

public class GetCoursesQuery : CursorQuery, IRequest<Result<CoursesResultDto>>
{
    public Guid? DepartmentId { get; set; }
    public string? SearchTerm { get; set; }

}
