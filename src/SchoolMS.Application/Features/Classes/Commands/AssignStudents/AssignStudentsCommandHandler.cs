using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Classes.Commands.AssignStudents;

public class AssignStudentsCommandHandler(IAppDbContext context, IUser user) : IRequestHandler<AssignStudentsCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(AssignStudentsCommand command, CancellationToken cancellationToken)
    {
        if (command.StudentIds == null || !command.StudentIds.Any())
            return ApplicationErrors.StudentIdsEmpty;


        var classEntity = await context.Classes
            .Include(c => c.StudentClasses)
            .FirstOrDefaultAsync(c => c.Id == command.ClassId && c.TeacherId == Guid.Parse(user.Id) && c.IsActive, cancellationToken);

        if (classEntity == null)
            return ClassErrors.NotFound;

        var students = await context.Users
            .Where(s => command.StudentIds.Contains(s.Id) && s.Role == Role.Student)
            .ToListAsync(cancellationToken);


        if (students.Count != command.StudentIds.Count)
        {
            var foundIds = students.Select(s => s.Id).ToHashSet();
            var missingIds = command.StudentIds.Where(id => !foundIds.Contains(id)).ToList();
            return Error.NotFound("Students.NotFound", $"Students not found: {string.Join(", ", missingIds)}");
        }


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
            }


        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success;

    }
}
