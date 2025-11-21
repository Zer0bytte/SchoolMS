namespace SchoolMS.Contracts.Classes;

public class CreateClassRequest
{
    public string Name { get; set; } = default!;
    public Guid CourseId { get; set; }
    public string Semester { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
