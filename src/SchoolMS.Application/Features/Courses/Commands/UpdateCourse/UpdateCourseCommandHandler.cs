using Microsoft.Extensions.Logging;
using SchoolMS.Application.Features.Courses.Dtos;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandHandler(
    IAppDbContext context,
    ILogger<UpdateCourseCommandHandler> logger
) : IRequestHandler<UpdateCourseCommand, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(UpdateCourseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Update course started. CourseId={CourseId}, Name={Name}, Code={Code}, DepartmentId={DepartmentId}, Credits={Credits}",
            command.Id, command.Name, command.Code, command.DepartmentId, command.Credits
        );

        var course = await context.Courses
            .Include(c => c.Department)
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (course is null)
        {
            logger.LogWarning(
                "Update course failed: course not found. CourseId={CourseId}",
                command.Id
            );
            return CourseErrors.NotFound;
        }

        var targetDepartmentId = command.DepartmentId ?? course.DepartmentId;

        var courseNameExist = await context.Courses
            .AnyAsync(
                c => c.Name == command.Name
                     && c.DepartmentId == targetDepartmentId
                     && c.Id != command.Id,
                cancellationToken);

        if (courseNameExist)
        {
            logger.LogWarning(
                "Update course failed: duplicate name in department. CourseId={CourseId}, DepartmentId={DepartmentId}, Name={Name}",
                command.Id, targetDepartmentId, command.Name
            );
            return CourseErrors.DuplicateName;
        }

        var codeExists = await context.Courses
            .AnyAsync(
                x => x.Code == command.Code
                     && x.DepartmentId == targetDepartmentId
                     && x.Id != command.Id,
                cancellationToken);

        if (codeExists)
        {
            logger.LogWarning(
                "Update course failed: duplicate code in department. CourseId={CourseId}, DepartmentId={DepartmentId}, Code={Code}",
                command.Id, targetDepartmentId, command.Code
            );
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

        logger.LogInformation(
            "Update course succeeded. CourseId={CourseId}, DepartmentId={DepartmentId}, Name={Name}, Code={Code}",
            courseDto.Id, courseDto.DepartmentId, courseDto.Name, courseDto.Code
        );

        return courseDto;
    }
}
