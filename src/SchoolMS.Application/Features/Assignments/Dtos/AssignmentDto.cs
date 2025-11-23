using SchoolMS.Application.Common.Models;

namespace SchoolMS.Application.Features.Assignments.Dtos;

public class AssignmentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateOnly DueDate { get; set; }
    public DateTimeOffset CreatedDateUtc { get; set; }

}