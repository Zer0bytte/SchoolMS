using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Submissions;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Assignments;

public sealed class Assignment : Entity
{
    public Guid ClassId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public DateOnly DueDate { get; private set; }
    public DateOnly CreatedDateUtc { get; private set; }
    public Guid CreatedByTeacherId { get; private set; }
    public Class Class { get; private set; } = default!;
    public User CreatedByTeacher { get; private set; } = default!;
    public ICollection<Submission> Submissions { get; private set; } = [];
    private Assignment(Guid id, Guid classId, string title, string description, DateOnly dueDate, DateOnly createdDateUtc, Guid createdByTeacherId) : base(id)
    {
        ClassId = classId;
        Title = title;
        Description = description;
        DueDate = dueDate;
        CreatedDateUtc = createdDateUtc;
        CreatedByTeacherId = createdByTeacherId;
    }

    public static Result<Assignment> Create(
      Guid id,
      Guid classId,
      string title,
      string description,
      DateOnly dueDate,
      DateOnly createdDateUtc,
      Guid createdByTeacherId,
      DateOnly todayUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
            return AssignmentErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(description))
            return AssignmentErrors.DescriptionRequired;

        if (dueDate <= todayUtc)
            return AssignmentErrors.DueDateMustBeInFuture;

        var assignment = new Assignment(
            id,
            classId,
            title,
            description,
            dueDate,
            createdDateUtc,
            createdByTeacherId
        );

        return assignment;
    }
}
