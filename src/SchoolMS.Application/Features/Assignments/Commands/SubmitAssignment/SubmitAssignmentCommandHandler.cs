using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Submissions;

namespace SchoolMS.Application.Features.Assignments.Commands.SubmitAssignment;

public class SubmitAssignmentCommandHandler(IAppDbContext context, IUser user, IFileStorageService fileStorage) : IRequestHandler<SubmitAssignmentCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(SubmitAssignmentCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
            return ApplicationErrors.UserNotFound;

        Guid studentId = Guid.Parse(user.Id);

        var assignment = await context.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == command.AssignmentId && a.Class.StudentClasses.Any(sc => sc.StudentId == Guid.Parse(user.Id)), cancellationToken);

        if (assignment is null)
            return AssignmentErrors.NotFound;

        bool alreadySubmitted = await context.Submissions
            .AnyAsync(s => s.AssignmentId == command.AssignmentId
                        && s.StudentId == studentId, cancellationToken);

        if (alreadySubmitted)
            return SubmissionErrors.AlreadYSubmitted;



        var fileUrl = await fileStorage.SaveFileAsync(command.File, "submissions");

        var submissionResult = Submission.Create(
            Guid.NewGuid(),
            command.AssignmentId,
            studentId,
            DateTimeOffset.UtcNow,
            fileUrl!
        );

        if (submissionResult.IsError)
            return submissionResult.Errors;

        var submission = submissionResult.Value;

        await context.Submissions.AddAsync(submission, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
