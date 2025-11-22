using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Queries.GetClassAttendance;

public class GetClassAttendanceQuery : IRequest<Result<List<StudentAttendanceEntry>>>
{
    public Guid ClassId { get; set; }
}
