namespace SchoolMS.Domain.Submissions;

public static class SubmissionErrors
{
    public static Error NotFound => Error.NotFound("Submission.NotFound", "Submission not found.");
    public static Error GradeLessThanZero => Error.Validation("Submission.Grade.LessThenZero", "The grade should be greater than or equal to zero.");
    public static Error RemarkRequired => Error.Validation("Submission.Remark.Required", "The remark is required.");
    public static Error InvalidFileUrl => Error.Validation("Submission.FileUrl.Invalid", "Invalid file url.");
    public static Error AlreadySubmitted => Error.Validation("Assignment.AlreadYSubmitted", "You already submitted this assignment.");
}