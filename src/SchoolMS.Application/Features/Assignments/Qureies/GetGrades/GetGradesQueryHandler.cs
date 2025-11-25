using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Features.Assignments.Dtos;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetGrades;

public class GetGradesQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetGradesQueryHandler> logger
) : IRequestHandler<GetGradesQuery, Result<List<GradeDto>>>
{
    public async Task<Result<List<GradeDto>>> Handle(GetGradesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning("Get grades failed: user id missing.");
            return new List<GradeDto>(); // or ApplicationErrors.UserNotFound if you prefer consistency
        }

        var studentId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get grades started. StudentId={StudentId}",
            studentId
        );

        var grades = await context.Submissions
            .Where(s => s.StudentId == studentId && s.Grade.HasValue)
            .Select(g => new GradeDto
            {
                AssignmentTitle = g.Assignment.Title,
                Grade = g.Grade!.Value,
                ClassName = g.Assignment.Class.Name,
                SolutionFileUrl = g.FileUrl,
                Remarks = g.Remarks
            })
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "Get grades succeeded. StudentId={StudentId}, ReturnedCount={ReturnedCount}",
            studentId, grades.Count
        );

        return grades;
    }
}
