using SchoolMS.Application.Features.Courses.Dtos;

namespace SchoolMS.Application.Features.Courses.Queries.GetCourseById;

public class GetCourseByIdQuery : IRequest<Result<CourseDto>>
{
    public Guid Id { get; set; }
}
