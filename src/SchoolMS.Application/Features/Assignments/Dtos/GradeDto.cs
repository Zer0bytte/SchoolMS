namespace SchoolMS.Application.Features.Assignments.Dtos;

public class GradeDto
{
    public string AssignmentTitle { get; set; } = default!;
    public string ClassName { get; set; } = default!;
    public decimal Grade { get; set; }
    public string? Remarks { get; set; } 
    public string SolutionFileUrl { get; set; } = default!;

}
