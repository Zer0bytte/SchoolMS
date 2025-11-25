using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Queries.Student.GetStudentAttendance;

public class GetStudentAttendanceQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetStudentAttendanceQueryHandler> logger
) : IRequestHandler<GetStudentAttendanceQuery, Result<List<ClassAttendanceDto>>>
{
    public async Task<Result<List<ClassAttendanceDto>>> Handle(GetStudentAttendanceQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning("Get student attendance failed: user id missing.");
            return ApplicationErrors.UserNotFound;
        }

        var studentId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get student attendance started. StudentId={StudentId}",
            studentId
        );

        var attendance = await context.Attendances
            .Where(a => a.StudentId == studentId)
            .Select(a => new ClassAttendanceDto
            {
                ClassId = a.ClassId,
                ClassName = a.Class.Name,
                Status = a.Status,
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Get student attendance succeeded. StudentId={StudentId}, ReturnedCount={ReturnedCount}",
            studentId, attendance.Count
        );

        return attendance;
    }
}
