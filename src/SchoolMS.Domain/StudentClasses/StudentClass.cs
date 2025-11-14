using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Common;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.StudentClasses;

public class StudentClass : Entity
{
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public DateTimeOffset EnrollmentDate { get; set; } 
    public User Student { get; set; } = default!;
    public Class Class { get; set; } = default!;
}