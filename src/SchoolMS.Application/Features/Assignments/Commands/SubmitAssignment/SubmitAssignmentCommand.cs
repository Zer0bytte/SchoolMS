namespace SchoolMS.Application.Features.Assignments.Commands.SubmitAssignment;

public class SubmitAssignmentCommand : IRequest<Result<Success>>
{
    public Guid AssignmentId { get; set; }
    public FileData File { get; set; }
}
