using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Domain.Assignments;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetAssignmentSubmissions;

public class GetAssignmentSubmissionsQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetAssignmentSubmissionsQuery, Result<List<SubmissionDto>>>
{
    public async Task<Result<List<SubmissionDto>>> Handle(GetAssignmentSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var assignment = await context.Assignments.AnyAsync(a => a.Id == request.AssignmentId && a.Class.TeacherId == Guid.Parse(user.Id));

        if (!assignment) return AssignmentErrors.NotFound;

        var submissions = await context.Submissions.Where(s => s.AssignmentId == request.AssignmentId).Select(s => new SubmissionDto
        {
            Id = s.Id,
            StudentName = s.Student.Name,
            FileUrl = s.FileUrl,
            Grade = s.Grade,
            IsGraded = s.Grade != null,
            Remarks = s.Remarks,
            SubmittedDate = s.SubmittedDate

        }).ToListAsync(cancellationToken);

        return submissions;
    }
}