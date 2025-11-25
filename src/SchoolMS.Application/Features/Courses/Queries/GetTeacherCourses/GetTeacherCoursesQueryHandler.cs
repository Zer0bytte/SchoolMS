using SchoolMS.Application.Features.Courses.Dtos;

namespace SchoolMS.Application.Features.Courses.Queries.GetTeacherCourses;

public class GetTeacherCoursesQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetTeacherCoursesQuery, Result<List<CourseDto>>>
{
    public async Task<Result<List<CourseDto>>> Handle(GetTeacherCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await context.Courses.Where(c => c.Department.HeadOfDepartmentId == Guid.Parse(user.Id))
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Credits = c.Credits,
                DepartmentId = c.DepartmentId,
                Description = c.Description,
                DepartmentName = c.Department.Name
            }).ToListAsync(cancellationToken);
        
        return courses;
    }
}
