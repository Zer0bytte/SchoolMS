using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Classes.Commands.AssignStudents;

public class AssignStudentsCommandHandler(
    IAppDbContext context,
    IUser user,
    ILogger<AssignStudentsCommandHandler> logger
) : IRequestHandler<AssignStudentsCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(AssignStudentsCommand command, CancellationToken cancellationToken)
    {
        if (command.StudentIds == null || !command.StudentIds.Any())
        {
            logger.LogWarning(
                "Assign students failed: empty student ids. ClassId={ClassId}, TeacherId={TeacherId}",
                command.ClassId, user.Id
            );
            return ApplicationErrors.StudentIdsEmpty;
        }

        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Assign students failed: user id missing. ClassId={ClassId}",
                command.ClassId
            );
            return ClassErrors.NotFound; 
        }

        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Assign students started. ClassId={ClassId}, TeacherId={TeacherId}, StudentIdsCount={StudentIdsCount}",
            command.ClassId, teacherId, command.StudentIds.Count
        );

        var classEntity = await context.Classes
            .Include(c => c.StudentClasses)
            .FirstOrDefaultAsync(
                c => c.Id == command.ClassId
                     && c.TeacherId == teacherId
                     && c.IsActive,
                cancellationToken);

        if (classEntity == null)
        {
            logger.LogWarning(
                "Assign students failed: class not found or not owned by teacher or inactive. ClassId={ClassId}, TeacherId={TeacherId}",
                command.ClassId, teacherId
            );
            return ClassErrors.NotFound;
        }

        var students = await context.Users
            .Where(s => command.StudentIds.Contains(s.Id) && s.Role == Role.Student)
            .ToListAsync(cancellationToken);

        if (students.Count != command.StudentIds.Count)
        {
            var foundIds = students.Select(s => s.Id).ToHashSet();
            var missingIds = command.StudentIds.Where(id => !foundIds.Contains(id)).ToList();

            logger.LogWarning(
                "Assign students failed: some students not found or not students. ClassId={ClassId}, TeacherId={TeacherId}, MissingIds={MissingIds}",
                command.ClassId, teacherId, missingIds
            );

            return Error.NotFound(
                "Students.NotFound",
                $"Students not found: {string.Join(", ", missingIds)}"
            );
        }

        int addedCount = 0;
        foreach (var student in students)
        {
            if (!classEntity.StudentClasses.Any(s => s.StudentId == student.Id))
            {
                classEntity.StudentClasses.Add(new StudentClass
                {
                    StudentId = student.Id,
                    ClassId = command.ClassId,
                    EnrollmentDate = DateTime.Now,
                });
                addedCount++;
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Assign students succeeded. ClassId={ClassId}, TeacherId={TeacherId}, AddedCount={AddedCount}, AlreadyEnrolledCount={AlreadyEnrolledCount}",
            command.ClassId, teacherId, addedCount, students.Count - addedCount
        );

        return Result.Success;
    }
}
