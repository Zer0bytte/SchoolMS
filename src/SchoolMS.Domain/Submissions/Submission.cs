using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Submissions;

public sealed class Submission : Entity
{
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public DateTimeOffset SubmittedDate { get; set; }
    public string FileUrl { get; set; } = default!;
    public decimal? Grade { get; set; }
    public Guid? GradedByTeacherId { get; set; }
    public string Remarks { get; set; } = default!;
    public Assignment Assignment { get; set; } = default!;
    public User Student { get; set; } = default!;
    public User GradedByTeacher { get; set; } = default!;
}