using SchoolMS.Application.Features.Courses.Dtos;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandHandler(IAppDbContext context) : IRequestHandler<UpdateCourseCommand, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
    {
        var course = await context.Courses
            .Include(c => c.Department)
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (course is null)
        {
            return CourseErrors.NotFound;
        }
        var targetDepartmentId = command.DepartmentId ?? course.DepartmentId;


        var courseNameExist = await context.Courses
            .AnyAsync(c => c.Name == command.Name
                        && c.DepartmentId == targetDepartmentId
                        && c.Id != command.Id,
                      cancellationToken);

        if (courseNameExist)
        {
            return CourseErrors.DuplicateName;
        }

        var codeExists = await context.Courses
            .AnyAsync(x => x.Code == command.Code
                        && x.DepartmentId == targetDepartmentId
                        && x.Id != command.Id,
                      cancellationToken);

        if (codeExists)
        {
            return CourseErrors.DuplicateCode;
        }

        course.Update(
            command.Name,
            command.Code,
            command.Description,
            targetDepartmentId,
            command.Credits);

        await context.SaveChangesAsync(cancellationToken);

        var courseDto = new CourseDto
        {
            Id = course.Id,
            Name = course.Name,
            Code = course.Code,
            Description = course.Description,
            DepartmentId = course.DepartmentId,
            DepartmentName = course.Department?.Name,
            Credits = course.Credits,
            CreatedDateUTC = course.CreatedDateUtc
        };

        return courseDto;
    }
}