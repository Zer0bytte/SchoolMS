using SchoolMS.Application.Features.Courses.Dtos;

namespace SchoolMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommand : IRequest<Result<CourseDto>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid? DepartmentId { get; set; }
    public int Credits { get; set; }
}