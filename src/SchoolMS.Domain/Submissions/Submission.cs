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
    public string? Remarks { get; set; } = default!;
    public Assignment Assignment { get; set; } = default!;
    public User Student { get; set; } = default!;
    public User GradedByTeacher { get; set; } = default!;

    private Submission(Guid id, Guid assignmentId, Guid studentId, DateTimeOffset submittedDate, string fileUrl) : base(id)
    {
        AssignmentId = assignmentId;
        StudentId = studentId;
        SubmittedDate = submittedDate;
        FileUrl = fileUrl;
    }


    public Result<Success> GradeAssignment(decimal grade, string remarks, Guid teacherId)
    {
        if (grade < 0)
            return SubmissionErrors.GradeLessThanZero;

        if (string.IsNullOrWhiteSpace(remarks))
            return SubmissionErrors.RemarkRequired;

        Grade = grade;
        Remarks = remarks;
        GradedByTeacherId = teacherId;
        return Result.Success;
    }


    public static Result<Submission> Create(Guid id, Guid assignmentId, Guid studentId, DateTimeOffset submittedDate, string fileUrl)
    {
        if (!IsValidUrl(fileUrl))
        {
            return SubmissionErrors.InvalidFileUrl;
        }


        return new Submission(id, assignmentId, studentId, submittedDate, fileUrl);
    }

    private static bool IsValidUrl(string fileUrl)
    {
        return Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
