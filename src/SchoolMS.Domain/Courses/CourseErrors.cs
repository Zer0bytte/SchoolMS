namespace SchoolMS.Domain.Courses;

public static class CourseErrors
{


    public static Error NotFound => Error.NotFound("Course.NotFound", "The specified course was not found.");
    public static Error DuplicateCode => Error.Conflict("Course.Code.Exists", "Course code is already exists.");
    public static Error DuplicateName => Error.Conflict("Course.Name.Exists", "Course name is already exists.");

    public static Error NameRequired =>
        Error.Validation("Course.Name.Required", "Course name is required.");

    public static Error CodeRequired =>
        Error.Validation("Course.Code.Required", "Course code is required.");

    public static Error DescriptionRequired =>
        Error.Validation("Course.Description.Required", "Course description is required.");

    public static Error DepartmentRequired =>
        Error.Validation("Course.Department.Required", "Department is required.");

    public static Error CreditsInvalid =>
        Error.Validation("Course.Credits.Invalid", "Credits must be greater than zero.");


}
