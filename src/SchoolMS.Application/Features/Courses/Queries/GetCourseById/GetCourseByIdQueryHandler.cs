using SchoolMS.Application.Features.Courses.Dtos;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Features.Courses.Queries.GetCourseById;

public class GetCourseByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetCourseByIdQuery, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery query, CancellationToken cancellationToken)
    {
        var course = await context.Courses
            .Where(c => c.Id == query.Id)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Description = c.Description,
                DepartmentId = c.DepartmentId,
                DepartmentName = c.Department.Name,
                Credits = c.Credits
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (course == null)
        {
            return CourseErrors.NotFound;
        }

        return course;
    }
}
