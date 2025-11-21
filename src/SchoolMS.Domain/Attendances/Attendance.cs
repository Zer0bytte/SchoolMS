using SchoolMS.Domain.Attendances.Enums;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Attendances;

public sealed class Attendance
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public Guid StudentId { get; set; }
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public Guid MarkedByTeacherId { get; set; }
    public Class Class { get; set; } = default!;
    public User Student { get; set; } = default!;
    public User MarkedByTeacher { get; set; } = default!;
}