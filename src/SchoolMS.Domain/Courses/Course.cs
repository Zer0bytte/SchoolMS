using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Domain.Courses;

public sealed class Course : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public int Credits { get; set; }
    public Department Department { get; set; } = default!;
    public ICollection<Class> Classes { get; set; } = [];

    private Course(Guid id, string name, string code, string description, Guid departmentId, int credits) : base(id)
    {
        Name = name;
        Code = code;
        Description = description;
        DepartmentId = departmentId;
        Credits = credits;
    }

    public static Result<Course> Create(Guid id, string name, string code, string description, Guid departmentId, int credits)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CourseErrors.NameRequired;

        if (string.IsNullOrWhiteSpace(code))
            return CourseErrors.CodeRequired;

        if (string.IsNullOrWhiteSpace(description))
            return CourseErrors.DescriptionRequired;

        if (departmentId == Guid.Empty)
            return CourseErrors.DepartmentRequired;

        if (credits <= 0)
            return CourseErrors.CreditsInvalid;

        var course = new Course(id, name, code, description, departmentId, credits);


        return course;
    }


    public void Update(string? name, string? code, string? description, Guid departmentId, int credits)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }

        if (!string.IsNullOrWhiteSpace(code))
        {
            Code = code;
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }

        if (departmentId != Guid.Empty)
        {
            DepartmentId = departmentId;
        }

        if (credits > 0)
        {
            Credits = credits;
        }
    }


    public void MarkAsDeleted()
    {
        IsActive = false;

    }

}