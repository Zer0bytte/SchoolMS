namespace SchoolMS.Contracts.Assignments;

public class GradeAssignmentRequest
{
    public decimal Grade { get; set; }
    public string Remarks { get; set; } = default!;
}
