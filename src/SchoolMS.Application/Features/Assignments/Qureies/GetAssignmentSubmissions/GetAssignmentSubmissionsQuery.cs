using SchoolMS.Application.Features.Assignments.Dtos;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetAssignmentSubmissions;

public class GetAssignmentSubmissionsQuery : IRequest<Result<List<SubmissionDto>>>
{
    public Guid AssignmentId { get; set; }
}
