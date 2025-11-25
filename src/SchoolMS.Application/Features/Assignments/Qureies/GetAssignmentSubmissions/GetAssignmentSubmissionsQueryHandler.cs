using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Domain.Assignments;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetAssignmentSubmissions;

public class GetAssignmentSubmissionsQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetAssignmentSubmissionsQueryHandler> logger
) : IRequestHandler<GetAssignmentSubmissionsQuery, Result<List<SubmissionDto>>>
{
    public async Task<Result<List<SubmissionDto>>> Handle(GetAssignmentSubmissionsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Get assignment submissions failed: user id missing. AssignmentId={AssignmentId}",
                request.AssignmentId
            );
            return ApplicationErrors.UserNotFound;
        }

        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get assignment submissions started. AssignmentId={AssignmentId}, TeacherId={TeacherId}",
            request.AssignmentId, teacherId
        );

        var assignmentExists = await context.Assignments
            .AnyAsync(a => a.Id == request.AssignmentId
                        && a.Class.TeacherId == teacherId,
                      cancellationToken);

        if (!assignmentExists)
        {
            logger.LogWarning(
                "Get assignment submissions failed: assignment not found or not owned by teacher. AssignmentId={AssignmentId}, TeacherId={TeacherId}",
                request.AssignmentId, teacherId
            );
            return AssignmentErrors.NotFound;
        }

        var submissions = await context.Submissions
            .Where(s => s.AssignmentId == request.AssignmentId)
            .Select(s => new SubmissionDto
            {
                Id = s.Id,
                StudentName = s.Student.Name,
                FileUrl = s.FileUrl,
                Grade = s.Grade,
                IsGraded = s.Grade != null,
                Remarks = s.Remarks,
                SubmittedDate = s.SubmittedDate
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Get assignment submissions succeeded. AssignmentId={AssignmentId}, TeacherId={TeacherId}, ReturnedCount={ReturnedCount}, GradedCount={GradedCount}",
            request.AssignmentId,
            teacherId,
            submissions.Count,
            submissions.Count(s => s.IsGraded)
        );

        return submissions;
    }
}
