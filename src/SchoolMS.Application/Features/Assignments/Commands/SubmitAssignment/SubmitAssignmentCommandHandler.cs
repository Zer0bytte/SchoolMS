using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Submissions;

namespace SchoolMS.Application.Features.Assignments.Commands.SubmitAssignment;

public class SubmitAssignmentCommandHandler(
    IAppDbContext context,
    IUser user,
    IFileStorageService fileStorage,
    ILogger<SubmitAssignmentCommandHandler> logger
) : IRequestHandler<SubmitAssignmentCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(SubmitAssignmentCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Submit assignment failed: user id missing. AssignmentId={AssignmentId}",
                command.AssignmentId
            );
            return ApplicationErrors.UserNotFound;
        }

        Guid studentId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Submitting assignment started. AssignmentId={AssignmentId}, StudentId={StudentId}",
            command.AssignmentId, studentId
        );

        var assignment = await context.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.Id == command.AssignmentId
                     && a.Class.StudentClasses.Any(sc => sc.StudentId == studentId),
                cancellationToken);

        if (assignment is null)
        {
            logger.LogWarning(
                "Submit assignment failed: assignment not found or student not enrolled. AssignmentId={AssignmentId}, StudentId={StudentId}",
                command.AssignmentId, studentId
            );
            return AssignmentErrors.NotFound;
        }

        bool alreadySubmitted = await context.Submissions
            .AnyAsync(
                s => s.AssignmentId == command.AssignmentId
                     && s.StudentId == studentId,
                cancellationToken);

        if (alreadySubmitted)
        {
            logger.LogWarning(
                "Submit assignment failed: already submitted. AssignmentId={AssignmentId}, StudentId={StudentId}",
                command.AssignmentId, studentId
            );
            return SubmissionErrors.AlreadySubmitted;
        }

        logger.LogInformation(
            "Saving submission file. AssignmentId={AssignmentId}, StudentId={StudentId}, FileName={FileName}",
            command.AssignmentId, studentId, command.File?.FileName
        );

        var fileUrl = await fileStorage.SaveFileAsync(command.File, "submissions");

        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            logger.LogError(
                "Submit assignment failed: file storage returned empty url. AssignmentId={AssignmentId}, StudentId={StudentId}",
                command.AssignmentId, studentId
            );
            return SubmissionErrors.FileUploadFailed;
        }

        var submissionResult = Submission.Create(
            Guid.NewGuid(),
            command.AssignmentId,
            studentId,
            DateTimeOffset.UtcNow,
            fileUrl!
        );

        if (submissionResult.IsError)
        {
            logger.LogWarning(
                "Submit assignment failed: domain validation errors. AssignmentId={AssignmentId}, StudentId={StudentId}, Errors={Errors}",
                command.AssignmentId, studentId, submissionResult.Errors
            );
            return submissionResult.Errors;
        }

        var submission = submissionResult.Value;

        await context.Submissions.AddAsync(submission, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Submitting assignment succeeded. AssignmentId={AssignmentId}, StudentId={StudentId}, SubmissionId={SubmissionId}",
            command.AssignmentId, studentId, submission.Id
        );

        return Result.Success;
    }
}
