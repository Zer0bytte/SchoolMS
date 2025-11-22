using SchoolMS.Application.Features.Assignments.Dtos;

namespace SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommand : IRequest<Result<AssignmentDto>>
{
    public Guid ClassId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateOnly DueDate { get; set; }
}
