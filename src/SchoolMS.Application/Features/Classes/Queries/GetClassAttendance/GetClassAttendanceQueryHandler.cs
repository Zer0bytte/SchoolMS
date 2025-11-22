using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Classes.Queries.GetClassAttendance;

public class GetClassAttendanceQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetClassAttendanceQuery, Result<List<StudentAttendanceEntry>>>
{
    public async Task<Result<List<StudentAttendanceEntry>>> Handle(GetClassAttendanceQuery request, CancellationToken cancellationToken)
    {
        var cls = await context.Classes.AnyAsync(c => c.Id == request.ClassId && c.TeacherId == Guid.Parse(user.Id));

        if (!cls)
        {
            return ClassErrors.NotFound;
        }

        var classAttendance = await context.Attendances
            .Where(a => a.ClassId == request.ClassId)
            .Select(a => new StudentAttendanceEntry
            {
                StudentId = a.StudentId,
                Status = a.Status
            })
            .ToListAsync(cancellationToken);


        return classAttendance;
    }
}
