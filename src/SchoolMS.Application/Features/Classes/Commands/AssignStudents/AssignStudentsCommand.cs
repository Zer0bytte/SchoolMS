namespace SchoolMS.Application.Features.Classes.Commands.AssignStudents;

public class AssignStudentsCommand : IRequest<Result<Success>>
{
    public Guid ClassId { get; set; }
    public List<Guid> StudentIds { get; set; } = [];
}
