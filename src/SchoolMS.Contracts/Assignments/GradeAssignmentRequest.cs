namespace SchoolMS.Contracts.Assignments;

public class GradeAssignmentRequest
{
    public Guid StudentId { get; set; }
    public decimal Grade { get; set; }
    public string Remarks { get; set; } = default!;
}
