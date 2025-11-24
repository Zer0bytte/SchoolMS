namespace SchoolMS.Domain.Assignments;

public static class AssignmentErrors
{
    public static Error NotFound =>
        Error.NotFound("Assignment.NotFound", "Assignment is not found.");



    public static Error TitleRequired =>
        Error.Validation("Assignment.TitleRequired", "Title is required.");

    public static Error DescriptionRequired =>
        Error.Validation("Assignment.DescriptionRequired", "Description is required.");

    public static Error DueDateMustBeInFuture =>
        Error.Validation("Assignment.DueDateFuture", "Due date must be in the future.");
}