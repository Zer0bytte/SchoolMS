using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetAssignments;

public class GetAssignmentsQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetAssignmentsQuery, Result<CursorResult<AssignmentDto>>>
{
    public async Task<Result<CursorResult<AssignmentDto>>> Handle(GetAssignmentsQuery query, CancellationToken cancellationToken)
    {

        var cls = await context.Classes.AnyAsync(c => c.Id == query.ClassId && c.TeacherId == Guid.Parse(user.Id));
        if (!cls) return ClassErrors.NotFound;

        var dbQuery = context.Assignments.Where(a => a.ClassId == query.ClassId).AsQueryable();

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
            .Select(d => new AssignmentDto
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                DueDate = d.DueDate

            })
            .ToListAsync(cancellationToken);

        var finalItems = items.Take(query.Limit).ToList();

        DateTimeOffset? nextDate = items.Count > query.Limit ? items[^1].CreatedDateUtc : null;
        Guid? nextId = items.Count > query.Limit ? items[^1].Id : null;

        var cursor = nextDate is not null && nextId is not null ? Cursor.Encode(nextDate.Value, nextId.Value) : null;
        var hasMore = items.Count > query.Limit;

        return CursorResult<AssignmentDto>.Create(cursor, hasMore, finalItems);
    }
}
