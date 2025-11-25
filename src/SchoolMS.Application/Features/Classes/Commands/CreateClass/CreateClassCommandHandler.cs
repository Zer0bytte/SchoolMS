using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommandHandler(
    IAppDbContext context,
    IUser user,
    ILogger<CreateClassCommandHandler> logger
) : IRequestHandler<CreateClassCommand, Result<ClassDto>>
{
    public async Task<Result<ClassDto>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Create class failed: user id missing. CourseId={CourseId}, Name={Name}",
                request.CourseId, request.Name
            );
            return ApplicationErrors.UserNotFound;
        }

        var headId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Create class started. CourseId={CourseId}, HeadId={HeadId}, Name={Name}, Semester={Semester}, StartDate={StartDate}, EndDate={EndDate}",
            request.CourseId, headId, request.Name, request.Semester, request.StartDate, request.EndDate
        );

        var course = await context.Courses
            .FirstOrDefaultAsync(
                c => c.Id == request.CourseId
                     && c.Department.HeadOfDepartmentId == headId,
                cancellationToken);

        if (course is null)
        {
            logger.LogWarning(
                "Create class failed: course not found or not owned by head. CourseId={CourseId}, HeadId={HeadId}",
                request.CourseId, headId
            );
            return CourseErrors.NotFound;
        }

        var clsExist = await context.Classes
            .AnyAsync(
                cls => cls.CourseId == request.CourseId && cls.Name == request.Name,
                cancellationToken);

        if (clsExist)
        {
            logger.LogWarning(
                "Create class failed: duplicate class name. CourseId={CourseId}, Name={Name}, HeadId={HeadId}",
                request.CourseId, request.Name, headId
            );
            return ClassErrors.DublicateName;
        }

        var newClass = Class.Create(
            Guid.CreateVersion7(),
            request.Name,
            request.CourseId,
            headId,
            request.Semester,
            request.StartDate,
            request.EndDate
        );

        if (newClass.IsError)
        {
            logger.LogWarning(
                "Create class failed: domain validation errors. CourseId={CourseId}, Name={Name}, HeadId={HeadId}, Errors={Errors}",
                request.CourseId, request.Name, headId, newClass.Errors
            );
            return newClass.Errors;
        }

        context.Classes.Add(newClass.Value);
        await context.SaveChangesAsync(cancellationToken);

        var classDto = new ClassDto
        {
            Id = newClass.Value.Id,
            Name = newClass.Value.Name,
            CourseId = newClass.Value.CourseId,
            CourseName = course.Name,
            Semester = newClass.Value.Semester,
            StartDate = newClass.Value.StartDate,
            EndDate = newClass.Value.EndDate,
        };

        logger.LogInformation(
            "Create class succeeded. ClassId={ClassId}, CourseId={CourseId}, HeadId={HeadId}, Name={Name}",
            classDto.Id, classDto.CourseId, headId, classDto.Name
        );

        return classDto;
    }
}
