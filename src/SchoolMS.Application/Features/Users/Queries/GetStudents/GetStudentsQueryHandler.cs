using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Users.Dtos;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Users.Queries.GetStudents;

public class GetStudentsQueryHandler(
    IAppDbContext context,
    ILogger<GetStudentsQueryHandler> logger
) : IRequestHandler<GetStudentsQuery, Result<CursorResult<StudentDto>>>
{
    public async Task<Result<CursorResult<StudentDto>>> Handle(GetStudentsQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Get students started. Cursor={Cursor}, Limit={Limit}",
            query.Cursor, query.Limit
        );

        var dbQuery = context.Users
            .Where(u => u.Role == Role.Student)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            logger.LogDebug(
                "Decoding cursor for students. Cursor={Cursor}",
                query.Cursor
            );

            var decodedCursor = Cursor.Decode(query.Cursor);
            if (decodedCursor is null)
            {
                logger.LogWarning(
                    "Get students failed: invalid cursor. Cursor={Cursor}",
                    query.Cursor
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
            .Select(d => new StudentDto
            {
                Id = d.Id,
                Name = d.Name,
                CreatedDateUtc = d.CreatedDateUtc
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
            "Get students succeeded. ReturnedCount={ReturnedCount}, HasMore={HasMore}, NextCursor={NextCursor}",
            finalItems.Count, hasMore, cursor
        );

        return CursorResult<StudentDto>.Create(cursor, hasMore, finalItems);
    }
}
