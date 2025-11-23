using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Assignments.Dtos;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetAssignments;

public class GetAssignmentsQuery : CursorQuery, IRequest<Result<CursorResult<AssignmentDto>>>
{
    public Guid ClassId { get; set; }
}
