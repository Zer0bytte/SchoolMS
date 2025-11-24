namespace SchoolMS.Application.Features.Assignments.Dtos;

public class SubmissionDto
{
    public Guid Id { get; set; }
    public string StudentName { get; set; } = default!;
    public bool IsGraded { get; set; }
    public DateTimeOffset SubmittedDate { get; set; }
    public string FileUrl { get; set; } = default!;
    public decimal? Grade { get; set; }
    public string? Remarks { get; set; } = default!;

}
