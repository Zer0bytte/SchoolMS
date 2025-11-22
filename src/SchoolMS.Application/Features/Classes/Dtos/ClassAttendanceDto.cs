using SchoolMS.Domain.Attendances.Enums;

namespace SchoolMS.Application.Features.Classes.Dtos;

public class ClassAttendanceDto
{
    public Guid StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
}
