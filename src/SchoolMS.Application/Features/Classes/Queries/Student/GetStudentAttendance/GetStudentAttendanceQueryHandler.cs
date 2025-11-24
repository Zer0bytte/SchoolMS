using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Queries.Student.GetStudentAttendance;

public class GetStudentAttendanceQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetStudentAttendanceQuery, Result<List<ClassAttendanceDto>>>
{
    public async Task<Result<List<ClassAttendanceDto>>> Handle(GetStudentAttendanceQuery query, CancellationToken cancellationToken)
    {
        var attendance = await context.Attendances.Where(a => a.StudentId == Guid.Parse(user.Id)).Select(a => new ClassAttendanceDto
        {
            ClassId = a.ClassId,
            ClassName = a.Class.Name,
            Status = a.Status,
        }).ToListAsync(cancellationToken);


        return attendance;
    }
}
