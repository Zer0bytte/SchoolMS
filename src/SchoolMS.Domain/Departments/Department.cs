using SchoolMS.Domain.Common;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Departments;

public sealed class Department : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid HeadOfDepartmentId { get; set; }
    public User HeadOfDepartment { get; set; } = default!;
    public ICollection<Course> Courses { get; set; } = [];


    private Department() { }

    public Department(Guid id, string name, string description, Guid headOfDepartmentId) : base(id)
    {
        Name = name;
        Description = description;
        HeadOfDepartmentId = headOfDepartmentId;
    }

    public static Result<Department> Create(Guid id, string name, string description, Guid headOfDepartmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DepartmentErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return DepartmentErrors.DescriptionRequired;
        }

        return new Department(id, name, description, headOfDepartmentId);
    }

    public void Update(string? name, string? description, Guid? headOfDepartmentId)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }

        if (headOfDepartmentId.HasValue)
        {
            HeadOfDepartmentId = headOfDepartmentId.Value;

        }
    }
}