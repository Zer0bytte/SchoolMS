using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Attendances;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Courses;
using SchoolMS.Domain.StudentClasses;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Classes;

public class Class : AuditableEntity
{
    public string Name { get; set; } = default!;
    public Guid CourseId { get; set; }
    public Guid TeacherId { get; set; }
    public string Semester { get; set; } = default!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public Course Course { get; set; } = default!;
    public User Teacher { get; set; } = default!;
    public ICollection<StudentClass> StudentClasses { get; set; } = [];
    public ICollection<Attendance> Attendances { get; set; } = [];
    public ICollection<Assignment> Assignments { get; set; } = [];

}
