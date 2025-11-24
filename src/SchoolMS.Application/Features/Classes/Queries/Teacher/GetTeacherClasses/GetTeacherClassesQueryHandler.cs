using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Queries.Teacher.GetTeacherClasses;

public class GetTeacherClassesQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetTeacherClassesQuery, Result<CursorResult<ClassDto>>>
{
    public async Task<Result<CursorResult<ClassDto>>> Handle(GetTeacherClassesQuery query, CancellationToken cancellationToken)
    {
        var dbQuery = context.Classes.Where(c => c.TeacherId == Guid.Parse(user.Id)).AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            var decodedCursor = Cursor.Decode(query.Cursor);
            if (decodedCursor is null)
            {
                return Error.Failure("InvalidCursor", "The provided cursor is invalid.");
            }
            dbQuery = dbQuery.Where(d => d.CreatedDateUtc < decodedCursor.Date ||
                                         d.CreatedDateUtc == decodedCursor.Date &&
                                         d.Id <= decodedCursor.LastId);
        }

        var items = await dbQuery
           .AsNoTracking()
           .OrderByDescending(d => d.CreatedDateUtc)
           .ThenByDescending(d => d.Id)
           .Take(query.Limit + 1)
           .Select(c => new ClassDto
           {
               Id = c.Id,
               Name = c.Name,
               CourseId = c.CourseId,
               CourseName = c.Course.Name,
               Semester = c.Semester,
               StartDate = c.StartDate,
               EndDate = c.EndDate,
               CreatedDateUtc = c.CreatedDateUtc,
           }).ToListAsync(cancellationToken);


        var finalItems = items.Take(query.Limit).ToList();

        DateTimeOffset? nextDate = items.Count > query.Limit ? items[^1].CreatedDateUtc : null;
        Guid? nextId = items.Count > query.Limit ? items[^1].Id : null;

        var cursor = nextDate is not null && nextId is not null ? Cursor.Encode(nextDate.Value, nextId.Value) : null;

        var hasMore = items.Count > query.Limit;

        var result = CursorResult<ClassDto>.Create(cursor, hasMore, finalItems);

        return result;
    }
}