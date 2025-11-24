using SchoolMS.Application.Features.Assignments.Dtos;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetStudentAssignments;

public class GetStudentAssignmentsQuery : IRequest<Result<List<StudentAssignmentDto>>>
{
    public Guid? ClassId { get; set; }
}
