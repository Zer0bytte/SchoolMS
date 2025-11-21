namespace SchoolMS.Contracts.Classes;

public class UpdateClassRequest
{
    public string Name { get; set; } = default!;
    public string Semester { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
