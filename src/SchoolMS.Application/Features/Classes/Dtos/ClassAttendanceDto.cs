using SchoolMS.Domain.Attendances.Enums;

namespace SchoolMS.Application.Features.Classes.Dtos;

public class ClassAttendanceDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = default!;
    public AttendanceStatus Status { get; set; }
}
