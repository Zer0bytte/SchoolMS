namespace SchoolMS.Application.Features.Classes.Dtos;

public class ClassDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = default!;
    public string Semester { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTimeOffset CreatedDateUtc { get; set; }
}
