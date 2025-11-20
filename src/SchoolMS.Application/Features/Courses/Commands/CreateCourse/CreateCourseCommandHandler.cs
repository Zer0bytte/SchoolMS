
using SchoolMS.Application.Features.Courses.Dtos;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandHandler(IAppDbContext context) : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(CreateCourseCommand command, CancellationToken ct)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(x => x.Id == command.DepartmentId, ct);

        if (department is null)
            return DepartmentErrors.NotFound;

        bool codeExists = await context.Courses
            .AnyAsync(x => x.Code == command.Code && x.DepartmentId == command.DepartmentId, ct);

        if (codeExists)
            return CourseErrors.DuplicateCode;

        bool nameExists = await context.Courses
            .AnyAsync(x => x.Name == command.Name && x.DepartmentId == command.DepartmentId, ct);

        if (nameExists)
            return CourseErrors.DuplicateName;

        var course = Course.Create(Guid.CreateVersion7(), command.Name, command.Code, command.Description, command.DepartmentId, command.Credits);

        if (course.IsError) return course.Errors;

        var courseObj = course.Value;

        context.Courses.Add(courseObj);
        await context.SaveChangesAsync(ct);

        var dto = new CourseDto
        {
            Id = courseObj.Id,
            Name = courseObj.Name,
            Code = courseObj.Code,
            Description = courseObj.Description,
            DepartmentId = courseObj.DepartmentId,
            DepartmentName = department.Name,
            Credits = courseObj.Credits,
            CreatedDateUTC = courseObj.CreatedDateUtc,
        };

        return dto;
    }
}