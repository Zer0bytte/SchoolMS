using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Features.Assignments.Commands.GradeAssignement;
using SchoolMS.Domain.Submissions;

public class GradeAssignmentCommandHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GradeAssignmentCommandHandler> logger
) : IRequestHandler<GradeAssignmentCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(GradeAssignmentCommand command, CancellationToken cancellationToken)
    {
        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Grading submission started. SubmissionId={SubmissionId}, TeacherId={TeacherId}, Grade={Grade}",
            command.SubmissionId, teacherId, command.Grade
        );

        Submission? submission = await context.Submissions
            .FirstOrDefaultAsync(
                s => s.Id == command.SubmissionId
                     && s.Assignment.CreatedByTeacherId == teacherId,
                cancellationToken);

        if (submission is null)
        {
            logger.LogWarning(
                "Submission not found or not owned by teacher. SubmissionId={SubmissionId}, TeacherId={TeacherId}",
                command.SubmissionId, teacherId
            );

            return SubmissionErrors.NotFound;
        }

        Result<Success> result = submission.GradeAssignment(
            command.Grade,
            command.Remarks,
            teacherId
        );

        if (result.IsError)
        {
            logger.LogWarning(
                "Grading submission failed due to domain errors. SubmissionId={SubmissionId}, TeacherId={TeacherId}, Errors={Errors}",
                command.SubmissionId, teacherId, result.Errors
            );

            return result.Errors;
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Grading submission succeeded. SubmissionId={SubmissionId}, TeacherId={TeacherId}",
            command.SubmissionId, teacherId
        );

        return Result.Success;
    }
}
