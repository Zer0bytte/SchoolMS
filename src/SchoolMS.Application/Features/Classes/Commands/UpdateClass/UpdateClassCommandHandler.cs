using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommandHandler(
    IAppDbContext context,
    IUser user,
    ILogger<UpdateClassCommandHandler> logger
) : IRequestHandler<UpdateClassCommand, Result<ClassDto>>
{
    public async Task<Result<ClassDto>> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Update class failed: user id missing. ClassId={ClassId}, Name={Name}",
                request.Id, request.Name
            );
            return ApplicationErrors.UserNotFound;
        }

        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Update class started. ClassId={ClassId}, TeacherId={TeacherId}, Name={Name}, Semester={Semester}, StartDate={StartDate}, EndDate={EndDate}",
            request.Id, teacherId, request.Name, request.Semester, request.StartDate, request.EndDate
        );

        var existingClass = await context.Classes
            .Include(c => c.Course)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive, cancellationToken);

        if (existingClass is null)
        {
            logger.LogWarning(
                "Update class failed: class not found or inactive. ClassId={ClassId}, TeacherId={TeacherId}",
                request.Id, teacherId
            );
            return ClassErrors.NotFound;
        }

        if (existingClass.TeacherId != teacherId)
        {
            logger.LogWarning(
                "Update class failed: teacher does not own class. ClassId={ClassId}, TeacherId={TeacherId}, OwnerTeacherId={OwnerTeacherId}",
                request.Id, teacherId, existingClass.TeacherId
            );
            return ClassErrors.NotFound;
        }

        var clsExist = await context.Classes
            .AnyAsync(
                cls => cls.Id != request.Id
                       && cls.Name == request.Name
                       && cls.CourseId == existingClass.CourseId,
                cancellationToken);

        if (clsExist)
        {
            logger.LogWarning(
                "Update class failed: duplicate class name in course. ClassId={ClassId}, CourseId={CourseId}, Name={Name}",
                request.Id, existingClass.CourseId, request.Name
            );
            return ClassErrors.DublicateName;
        }

        var updateResult = existingClass.Update(
            request.Name,
            request.Semester,
            request.StartDate,
            request.EndDate);

        if (updateResult.IsError)
        {
            logger.LogWarning(
                "Update class failed: domain validation errors. ClassId={ClassId}, TeacherId={TeacherId}, Errors={Errors}",
                request.Id, teacherId, updateResult.Errors
            );
            return updateResult.Errors;
        }

        await context.SaveChangesAsync(cancellationToken);

        var dto = new ClassDto
        {
            Id = existingClass.Id,
            Name = existingClass.Name,
            CourseId = existingClass.CourseId,
            CourseName = existingClass.Course.Name,
            Semester = existingClass.Semester,
            StartDate = existingClass.StartDate,
            EndDate = existingClass.EndDate
        };

        logger.LogInformation(
            "Update class succeeded. ClassId={ClassId}, TeacherId={TeacherId}, Name={Name}",
            dto.Id, teacherId, dto.Name
        );

        return dto;
    }
}
