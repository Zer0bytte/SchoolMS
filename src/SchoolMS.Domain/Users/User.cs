using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Submissions;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Domain.Users;

public class User : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Role Role { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Department> ManagedDepartments { get; set; } = [];
    public ICollection<Class> TaughtClasses { get; set; } = [];
    public ICollection<StudentClass> StudentClasses { get; set; } = [];
    public ICollection<Attendance> Attendances { get; set; } = [];
    public ICollection<Submission> Submissions { get; set; } = [];
}
