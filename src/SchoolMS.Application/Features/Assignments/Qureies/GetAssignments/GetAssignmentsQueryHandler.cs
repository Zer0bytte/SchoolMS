using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Assignments.Qureies.GetAssignments;

public class GetAssignmentsQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetAssignmentsQueryHandler> logger
) : IRequestHandler<GetAssignmentsQuery, Result<CursorResult<AssignmentDto>>>
{
    public async Task<Result<CursorResult<AssignmentDto>>> Handle(GetAssignmentsQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Get assignments failed: user id missing. ClassId={ClassId}",
                query.ClassId
            );
            return ClassErrors.NotFound;
        }

        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get assignments started. ClassId={ClassId}, TeacherId={TeacherId}, Cursor={Cursor}, Limit={Limit}",
            query.ClassId, teacherId, query.Cursor, query.Limit
        );

        var cls = await context.Classes
            .AnyAsync(c => c.Id == query.ClassId && c.TeacherId == teacherId, cancellationToken);

        if (!cls)
        {
            logger.LogWarning(
                "Get assignments failed: class not found or not owned by teacher. ClassId={ClassId}, TeacherId={TeacherId}",
                query.ClassId, teacherId
            );
            return ClassErrors.NotFound;
        }

        var dbQuery = context.Assignments
            .Where(a => a.ClassId == query.ClassId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            logger.LogDebug(
                "Decoding cursor for assignments. ClassId={ClassId}, TeacherId={TeacherId}, Cursor={Cursor}",
                query.ClassId, teacherId, query.Cursor
            );

            var decodedCursor = Cursor.Decode(query.Cursor);
            if (decodedCursor is null)
            {
                logger.LogWarning(
                    "Get assignments failed: invalid cursor. ClassId={ClassId}, TeacherId={TeacherId}, Cursor={Cursor}",
                    query.ClassId, teacherId, query.Cursor
                );

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

        var cursor = nextDate is not null && nextId is not null
            ? Cursor.Encode(nextDate.Value, nextId.Value)
            : null;

        var hasMore = items.Count > query.Limit;

        logger.LogInformation(
            "Get assignments succeeded. ClassId={ClassId}, TeacherId={TeacherId}, ReturnedCount={ReturnedCount}, HasMore={HasMore}, NextCursor={NextCursor}",
            query.ClassId, teacherId, finalItems.Count, hasMore, cursor
        );

        return CursorResult<AssignmentDto>.Create(cursor, hasMore, finalItems);
    }
}
