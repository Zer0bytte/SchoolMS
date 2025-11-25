using Microsoft.Extensions.Logging;
using SchoolMS.Application.Features.Assignments.Dtos;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetStudentAssignments;

public class GetStudentAssignmentsQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetStudentAssignmentsQueryHandler> logger
) : IRequestHandler<GetStudentAssignmentsQuery, Result<List<StudentAssignmentDto>>>
{
    public async Task<Result<List<StudentAssignmentDto>>> Handle(GetStudentAssignmentsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Get student assignments failed: user id missing. ClassId={ClassId}",
                request.ClassId
            );
            return new List<StudentAssignmentDto>();
        }

        var studentId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get student assignments started. StudentId={StudentId}, ClassId={ClassId}",
            studentId, request.ClassId
        );

        var dbQuery = context.Assignments.AsQueryable();

        if (request.ClassId.HasValue && request.ClassId.Value != Guid.Empty)
        {
            logger.LogDebug(
                "Filtering student assignments by class. StudentId={StudentId}, ClassId={ClassId}",
                studentId, request.ClassId
            );

            dbQuery = dbQuery.Where(a => a.ClassId == request.ClassId);
        }

        var assignments = await dbQuery
            .Where(a => a.Class.StudentClasses.Any(sc => sc.StudentId == studentId))
            .Select(a => new StudentAssignmentDto
            {
                Id = a.Id,
                ClassName = a.Class.Name,
                Description = a.Description,
                DueDate = a.DueDate,
                Title = a.Title,
                Submitted = a.Submissions.Any(s => s.StudentId == studentId),
                SubmissionFileUrl = a.Submissions
                    .Where(s => s.StudentId == studentId)
                    .Select(s => s.FileUrl)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Get student assignments succeeded. StudentId={StudentId}, ClassId={ClassId}, ReturnedCount={ReturnedCount}, SubmittedCount={SubmittedCount}",
            studentId,
            request.ClassId,
            assignments.Count,
            assignments.Count(a => a.Submitted)
        );

        return assignments;
    }
}
