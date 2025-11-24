namespace SchoolMS.Contracts.Assignments;

public class CreateAssignmentRequest
{
    public Guid ClassId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateOnly DueDate { get; set; }
}
