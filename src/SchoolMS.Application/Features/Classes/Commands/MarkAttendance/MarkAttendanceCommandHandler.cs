using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Classes.Commands.MarkAttendance;

public class MarkAttendanceCommandHandler(
    IAppDbContext context,
    IUser user,
    ILogger<MarkAttendanceCommandHandler> logger
) : IRequestHandler<MarkAttendanceCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(MarkAttendanceCommand command, CancellationToken cancellationToken)
    {
        if (command.Students == null || !command.Students.Any())
        {
            logger.LogWarning(
                "Mark attendance failed: empty students list. ClassId={ClassId}, TeacherId={TeacherId}",
                command.ClassId, user.Id
            );
            return ApplicationErrors.StudentIdsEmpty;
        }

        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Mark attendance failed: user id missing. ClassId={ClassId}",
                command.ClassId
            );
            return ClassErrors.NotFound;
        }

        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Mark attendance started. ClassId={ClassId}, TeacherId={TeacherId}, StudentsCount={StudentsCount}",
            command.ClassId, teacherId, command.Students.Count
        );

        var classEntity = await context.Classes
            .Include(c => c.StudentClasses)
            .Include(c => c.Attendances)
            .FirstOrDefaultAsync(
                c => c.Id == command.ClassId
                     && c.TeacherId == teacherId
                     && c.IsActive,
                cancellationToken);

        if (classEntity == null)
        {
            logger.LogWarning(
                "Mark attendance failed: class not found / not owned / inactive. ClassId={ClassId}, TeacherId={TeacherId}",
                command.ClassId, teacherId
            );
            return ClassErrors.NotFound;
        }

        var assignedStudents = classEntity.StudentClasses.Select(s => s.StudentId).ToHashSet();
        command.Students = command.Students.DistinctBy(s => s.StudentId).ToList();
        var studentIds = command.Students.DistinctBy(s => s.StudentId).Select(s => s.StudentId).ToList();

        var missingIds = studentIds.Where(id => !assignedStudents.Contains(id)).ToList();
        if (missingIds.Any())
        {
            logger.LogWarning(
                "Mark attendance failed: some students not assigned to class. ClassId={ClassId}, TeacherId={TeacherId}, MissingIds={MissingIds}",
                command.ClassId, teacherId, missingIds
            );
            return Error.NotFound("Students.NotFound", "Students not found.");
        }

        var attendanceDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var alreadyMarkedIds = await context.Attendances
            .Where(a => a.ClassId == command.ClassId
                        && a.Date == attendanceDate
                        && studentIds.Contains(a.StudentId))
            .Select(a => a.StudentId)
            .ToListAsync(cancellationToken);

        if (alreadyMarkedIds.Any())
        {
            logger.LogWarning(
                "Mark attendance failed: already marked for some students. ClassId={ClassId}, TeacherId={TeacherId}, Date={Date}, AlreadyMarkedIds={AlreadyMarkedIds}",
                command.ClassId, teacherId, attendanceDate, alreadyMarkedIds
            );

            return Error.Conflict(
                "Attendance.AlreadyMarked",
                "Attendance is already marked today for some students."
            );
        }

        foreach (var student in command.Students)
        {
            var attendanceResult = Attendance.Create(
                Guid.CreateVersion7(),
                command.ClassId,
                student.StudentId,
                attendanceDate,
                student.Status,
                teacherId
            );

            if (attendanceResult.IsError)
            {
                logger.LogWarning(
                    "Mark attendance failed: domain validation error. ClassId={ClassId}, TeacherId={TeacherId}, StudentId={StudentId}, Status={Status}, Errors={Errors}",
                    command.ClassId, teacherId, student.StudentId, student.Status, attendanceResult.Errors
                );
                return attendanceResult.Errors;
            }

            context.Attendances.Add(attendanceResult.Value);
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Mark attendance succeeded. ClassId={ClassId}, TeacherId={TeacherId}, Date={Date}, MarkedCount={MarkedCount}",
            command.ClassId, teacherId, attendanceDate, command.Students.Count
        );

        return Result.Success;
    }

}
