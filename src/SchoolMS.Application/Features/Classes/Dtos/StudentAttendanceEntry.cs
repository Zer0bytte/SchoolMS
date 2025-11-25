using SchoolMS.Domain.Attendances.Enums;
using System.Text;

namespace SchoolMS.Application.Features.Classes.Dtos;

public class StudentAttendanceEntry
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = default!;
    public AttendanceStatus Status { get; set; }
}
