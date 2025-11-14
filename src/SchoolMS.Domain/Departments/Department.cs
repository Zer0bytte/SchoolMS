using SchoolMS.Domain.Common;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Departments;

public class Department : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid HeadOfDepartmentId { get; set; }
    public User HeadOfDepartment { get; set; } = default!;
    public ICollection<Course> Courses { get; set; } = [];
}