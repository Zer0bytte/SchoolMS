using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Domain.Courses;

public class Course : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;

    public int Credits { get; set; }
    public Department Department { get; set; } = default!;
    public ICollection<Class> Classes { get; set; } = [];
}