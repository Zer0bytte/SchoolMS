using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommand : IRequest<Result<ClassDto>>
{
    public string Name { get; set; } = default!;
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
