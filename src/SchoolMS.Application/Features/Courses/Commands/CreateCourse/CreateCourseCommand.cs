using SchoolMS.Application.Features.Courses.Dtos;

namespace SchoolMS.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommand : IRequest<Result<CourseDto>>
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public int Credits { get; set; }
}
