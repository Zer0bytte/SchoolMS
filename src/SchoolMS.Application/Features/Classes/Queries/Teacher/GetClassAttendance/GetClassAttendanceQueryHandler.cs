using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Classes.Queries.Teacher.GetClassAttendance;

public class GetClassAttendanceQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetClassAttendanceQueryHandler> logger
) : IRequestHandler<GetClassAttendanceQuery, Result<List<StudentAttendanceEntry>>>
{
    public async Task<Result<List<StudentAttendanceEntry>>> Handle(GetClassAttendanceQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Get class attendance failed: user id missing. ClassId={ClassId}",
                request.ClassId
            );
            return ApplicationErrors.UserNotFound;
        }

        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get class attendance started. ClassId={ClassId}, TeacherId={TeacherId}",
            request.ClassId, teacherId
        );

        var clsExists = await context.Classes
            .AnyAsync(c => c.Id == request.ClassId && c.TeacherId == teacherId, cancellationToken);

        if (!clsExists)
        {
            logger.LogWarning(
                "Get class attendance failed: class not found or not owned by teacher. ClassId={ClassId}, TeacherId={TeacherId}",
                request.ClassId, teacherId
            );
            return ClassErrors.NotFound;
        }

        var classAttendance = await context.Attendances
            .Where(a => a.ClassId == request.ClassId)
            .Select(a => new StudentAttendanceEntry
            {
                StudentId = a.StudentId,
                StudentName = a.Student.Name,
                Status = a.Status
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Get class attendance succeeded. ClassId={ClassId}, TeacherId={TeacherId}, ReturnedCount={ReturnedCount}",
            request.ClassId, teacherId, classAttendance.Count
        );

        return classAttendance;
    }
}
