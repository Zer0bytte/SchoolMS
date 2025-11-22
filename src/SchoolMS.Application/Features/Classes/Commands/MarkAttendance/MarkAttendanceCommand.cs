using SchoolMS.Domain.Attendances.Enums;

namespace SchoolMS.Application.Features.Classes.Commands.MarkAttendance;

public class MarkAttendanceCommand : IRequest<Result<Success>>
{
    public Guid ClassId { get; set; }
    public List<StudentAttendanceEntry> Students { get; set; } = [];
}


public class StudentAttendanceEntry
{
    public Guid StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
}