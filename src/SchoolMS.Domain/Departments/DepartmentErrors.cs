namespace SchoolMS.Domain.Departments;

public static class DepartmentErrors
{
    public static Error NameRequired => Error.Validation("Department.Name.Required", "Department name is required.");
    public static Error DescriptionRequired => Error.Validation("Department.Description.Required", "Department description is required.");
    public static Error DublicateName => Error.Conflict("Department.Name.Exists", "A department with the same name already exists.");
    public static Error NotFound => Error.NotFound("Department.NotFound", "The specified department was not found.");
}
