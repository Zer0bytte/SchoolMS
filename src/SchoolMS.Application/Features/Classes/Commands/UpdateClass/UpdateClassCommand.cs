using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommand : IRequest<Result<ClassDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Semester { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
