using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Courses.Dtos;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandHandler(
    IAppDbContext context,
    IUser user,
    ILogger<CreateCourseCommandHandler> logger,
    HybridCache cache
) : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(CreateCourseCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Create course failed: user id missing. DepartmentId={DepartmentId}, Name={Name}, Code={Code}",
                command.DepartmentId, command.Name, command.Code
            );
            return ApplicationErrors.UserNotFound;
        }

        var headId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Create course started. DepartmentId={DepartmentId}, HeadId={HeadId}, Name={Name}, Code={Code}, Credits={Credits}",
            command.DepartmentId, headId, command.Name, command.Code, command.Credits
        );

        var department = await context.Departments
            .FirstOrDefaultAsync(
                x => x.Id == command.DepartmentId && x.HeadOfDepartmentId == headId,
                ct);

        if (department is null)
        {
            logger.LogWarning(
                "Create course failed: department not found or not owned by head. DepartmentId={DepartmentId}, HeadId={HeadId}",
                command.DepartmentId, headId
            );
            return DepartmentErrors.NotFound;
        }

        bool codeExists = await context.Courses
            .AnyAsync(x => x.Code == command.Code && x.DepartmentId == command.DepartmentId, ct);

        if (codeExists)
        {
            logger.LogWarning(
                "Create course failed: duplicate code in department. DepartmentId={DepartmentId}, Code={Code}, HeadId={HeadId}",
                command.DepartmentId, command.Code, headId
            );
            return CourseErrors.DuplicateCode;
        }

        bool nameExists = await context.Courses
            .AnyAsync(x => x.Name == command.Name && x.DepartmentId == command.DepartmentId, ct);

        if (nameExists)
        {
            logger.LogWarning(
                "Create course failed: duplicate name in department. DepartmentId={DepartmentId}, Name={Name}, HeadId={HeadId}",
                command.DepartmentId, command.Name, headId
            );
            return CourseErrors.DuplicateName;
        }

        var course = Course.Create(
            Guid.CreateVersion7(),
            command.Name,
            command.Code,
            command.Description,
            command.DepartmentId,
            command.Credits
        );

        if (course.IsError)
        {
            logger.LogWarning(
                "Create course failed: domain validation errors. DepartmentId={DepartmentId}, Name={Name}, Code={Code}, HeadId={HeadId}, Errors={Errors}",
                command.DepartmentId, command.Name, command.Code, headId, course.Errors
            );
            return course.Errors;
        }

        var courseObj = course.Value;

        context.Courses.Add(courseObj);
        await context.SaveChangesAsync(ct);
        await cache.RemoveByTagAsync("courses");
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

        logger.LogInformation(
            "Create course succeeded. CourseId={CourseId}, DepartmentId={DepartmentId}, HeadId={HeadId}, Code={Code}, Name={Name}",
            dto.Id, dto.DepartmentId, headId, dto.Code, dto.Name
        );

        return dto;
    }
}
