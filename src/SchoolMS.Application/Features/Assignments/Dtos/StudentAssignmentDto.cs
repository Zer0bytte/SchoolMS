namespace SchoolMS.Application.Features.Assignments.Dtos;

public class StudentAssignmentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ClassName { get; set; } = default!;
    public DateOnly DueDate { get; set; }
    public bool Submitted { get; set; }
    public string? SubmissionFileUrl { get; set; }

}
