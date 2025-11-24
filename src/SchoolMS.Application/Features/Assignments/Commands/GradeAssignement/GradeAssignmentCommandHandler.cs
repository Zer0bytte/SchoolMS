using SchoolMS.Domain.Submissions;

namespace SchoolMS.Application.Features.Assignments.Commands.GradeAssignement;

public class GradeAssignmentCommandHandler(IAppDbContext context, IUser user) : IRequestHandler<GradeAssignmentCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(GradeAssignmentCommand command, CancellationToken cancellationToken)
    {
        Submission? submission = await context.Submissions
            .FirstOrDefaultAsync(
            s => s.Id == command.SubmissionId
            && s.Assignment.CreatedByTeacherId == Guid.Parse(user.Id), cancellationToken);

        if (submission is null)
            return SubmissionErrors.NotFound;

        Result<Success> result = submission.GradeAssignment(command.Grade, command.Remarks, Guid.Parse(user.Id));

        if (result.IsError) return result.Errors;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}