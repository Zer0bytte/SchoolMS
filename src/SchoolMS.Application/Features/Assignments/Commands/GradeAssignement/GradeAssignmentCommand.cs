namespace SchoolMS.Application.Features.Assignments.Commands.GradeAssignement;

public class GradeAssignmentCommand : IRequest<Result<Success>>
{
    public Guid SubmissionId { get; set; }
    public Guid StudentId { get; set; }
    public decimal Grade { get; set; }
    public string Remarks { get; set; } = default!;
}
