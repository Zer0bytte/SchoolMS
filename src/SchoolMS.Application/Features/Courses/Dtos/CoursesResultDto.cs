using SchoolMS.Application.Common.Models;

namespace SchoolMS.Application.Features.Courses.Dtos;

public class CoursesResultDto : CurosrResult
{
    public List<CourseDto> Items { get; set; } = [];
}
