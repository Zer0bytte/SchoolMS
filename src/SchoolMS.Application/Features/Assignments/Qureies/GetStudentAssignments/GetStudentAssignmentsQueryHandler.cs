using SchoolMS.Application.Features.Assignments.Dtos;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetStudentAssignments;

public class GetStudentAssignmentsQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetStudentAssignmentsQuery, Result<List<StudentAssignmentDto>>>
{
    public async Task<Result<List<StudentAssignmentDto>>> Handle(GetStudentAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var dbQuery = context.Assignments.AsQueryable();

        if (request.ClassId.HasValue && request.ClassId.Value != Guid.Empty)
        {
            dbQuery = dbQuery.Where(a => a.ClassId == request.ClassId);
        }

        var studentId = Guid.Parse(user.Id!);

        var assignments = await dbQuery
            .Where(a => a.Class.StudentClasses.Any(sc => sc.StudentId == studentId))
            .Select(a => new StudentAssignmentDto
            {
                Id = a.Id,
                ClassName = a.Class.Name,
                Description = a.Description,
                DueDate = a.DueDate,
                Title = a.Title,
                Submitted = a.Submissions.Any(s => s.StudentId == studentId),
                SubmissionFileUrl = a.Submissions
                    .Where(s => s.StudentId == studentId)
                    .Select(s => s.FileUrl)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);


        return assignments;
    }
}