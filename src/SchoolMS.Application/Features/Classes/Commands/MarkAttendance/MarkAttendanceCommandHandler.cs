using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Classes.Commands.MarkAttendance;

public class MarkAttendanceCommandHandler(IAppDbContext context, IUser user) : IRequestHandler<MarkAttendanceCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(MarkAttendanceCommand command, CancellationToken cancellationToken)
    {

        if (command.Students == null || !command.Students.Any())
            return ApplicationErrors.StudentIdsEmpty;


        var classEntity = await context.Classes
            .Include(c => c.StudentClasses)
            .FirstOrDefaultAsync(c => c.Id == command.ClassId && c.TeacherId == Guid.Parse(user.Id) && c.IsActive, cancellationToken);

        if (classEntity == null)
            return ClassErrors.NotFound;

        var studentIds = command.Students.Select(s => s.StudentId).ToList();

        var students = await context.Users
            .Where(s => studentIds.Contains(s.Id) && s.Role == Role.Student)
            .ToListAsync(cancellationToken);


        if (students.Count != command.Students.Count)
        {
            var foundIds = students.Select(s => s.Id).ToHashSet();
            var missingIds = studentIds.Where(id => !foundIds.Contains(id)).ToList();
            return Error.NotFound("Students.NotFound", $"Students not found: {string.Join(", ", missingIds)}");
        }

        foreach (var student in command.Students)
        {
            var attendance = Attendance.Create(
                                            Guid.CreateVersion7(),
                                            command.ClassId,
                                            student.StudentId,
                                            DateOnly.FromDateTime(DateTime.UtcNow),
                                            student.Status,
                                            Guid.Parse(user.Id)).Value;

            context.Attendances.Add(attendance);
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;



    }
}
