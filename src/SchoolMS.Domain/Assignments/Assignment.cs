using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Submissions;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Assignments;

public sealed class Assignment : Entity
{
    public Guid ClassId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTimeOffset DueDate { get; set; }
    public DateTimeOffset CreatedDateUtc { get; set; }
    public Guid CreatedByTeacherId { get; set; }
    public Class Class { get; set; } = default!;
    public User CreatedByTeacher { get; set; } = default!;
    public ICollection<Submission> Submissions { get; set; } = [];
}