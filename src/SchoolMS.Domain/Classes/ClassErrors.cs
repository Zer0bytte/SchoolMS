namespace SchoolMS.Domain.Classes;

public static class ClassErrors
{
    public static Error DublicateName => Error.Conflict("Class.Name.Exists", "A class with the same name already exists.");
    public static Error NameRequired => Error.Validation("Class.Name.Required", "Class name is required.");
    public static Error NotFound => Error.NotFound("Class.NotFound", "The class with the given ID could not be found");
    public static Error CourseIdInvalid => Error.Validation("Class.CourseId.Invalid", "CourseId is invalid.");
    public static Error TeacherIdInvalid => Error.Validation("Class.TeacherId.Invalid", "TeacherId is invalid.");
    public static Error SemesterRequired => Error.Validation("Class.Semester.Required", "Semester is required.");
    public static Error StartDateInvalid => Error.Validation("Class.StartDate.Invalid", "StartDate is invalid.");
    public static Error EndDateInvalid => Error.Validation("Class.EndDate.Invalid", "EndDate is invalid.");
    public static Error EndDateBeforeStartDate => Error.Validation("Class.EndDate.BeforeStartDate", "EndDate cannot be before StartDate.");

}
