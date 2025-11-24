using SchoolMS.Application.Features.Assignments.Dtos;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetGrades;

public class GetGradesQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetGradesQuery, Result<List<GradeDto>>>
{
    public async Task<Result<List<GradeDto>>> Handle(GetGradesQuery request, CancellationToken cancellationToken)
    {
        var grades = await context.Submissions.Where(s => s.StudentId == Guid.Parse(user.Id) && s.Grade.HasValue).Select(
          g => new GradeDto
          {
              AssignmentTitle = g.Assignment.Title,
              Grade = g.Grade!.Value,
              ClassName = g.Assignment.Class.Name,
              SolutionFileUrl = g.FileUrl,
              Remarks = g.Remarks
          }).ToListAsync(cancellationToken);

        return grades;
    }
}