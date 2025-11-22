using SchoolMS.Domain.Attendances.Enums;

namespace SchoolMS.Application.Features.Classes.Dtos;

public class StudentAttendanceEntry
{
    public Guid StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
}
