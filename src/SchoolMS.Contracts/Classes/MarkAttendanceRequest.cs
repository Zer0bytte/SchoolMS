using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Contracts.Classes;

public class MarkAttendanceRequest
{
    public Guid ClassId { get; set; }
    public List<StudentAttendanceEntry> Students { get; set; } = [];
}
