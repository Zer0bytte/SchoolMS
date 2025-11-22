using SchoolMS.Domain.Attendances.Enums;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Attendances;

public sealed class Attendance
{
    public Guid Id { get; private set; }
    public Guid ClassId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateOnly Date { get; private set; }
    public AttendanceStatus Status { get; private set; }
    public Guid MarkedByTeacherId { get; private set; }
    public Class Class { get; private set; } = default!;
    public User Student { get; private set; } = default!;
    public User MarkedByTeacher { get; private set; } = default!;

    private Attendance(Guid id, Guid classId, Guid studentId, DateOnly date, AttendanceStatus status, Guid markedByTeacherId)
    {
        Id = id;
        ClassId = classId;
        StudentId = studentId;
        Date = date;
        Status = status;
        MarkedByTeacherId = markedByTeacherId;
    }


    public static Result<Attendance> Create(Guid id, Guid classId, Guid studentId, DateOnly date, AttendanceStatus status, Guid markedByTeacherId)
    {
        return new Attendance(id, classId, studentId, date, status, markedByTeacherId);
    }
}