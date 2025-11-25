using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Queries.Student.GetStudentClasses;

public class GetStudentClassesQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetStudentClassesQueryHandler> logger
) : IRequestHandler<GetStudentClassesQuery, Result<CursorResult<ClassDto>>>
{
    public async Task<Result<CursorResult<ClassDto>>> Handle(GetStudentClassesQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Get student classes failed: user id missing. Cursor={Cursor}, Limit={Limit}",
                query.Cursor, query.Limit
            );
            return ApplicationErrors.UserNotFound;
        }

        var studentId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get student classes started. StudentId={StudentId}, Cursor={Cursor}, Limit={Limit}",
            studentId, query.Cursor, query.Limit
        );

        var dbQuery = context.Classes
            .Where(c => c.StudentClasses.Any(sc => sc.StudentId == studentId))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            logger.LogDebug(
                "Decoding cursor for student classes. StudentId={StudentId}, Cursor={Cursor}",
                studentId, query.Cursor
            );

            var decodedCursor = Cursor.Decode(query.Cursor);
            if (decodedCursor is null)
            {
                logger.LogWarning(
                    "Get student classes failed: invalid cursor. StudentId={StudentId}, Cursor={Cursor}",
                    studentId, query.Cursor
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
            })
            .ToListAsync(cancellationToken);

        var finalItems = items.Take(query.Limit).ToList();

        DateTimeOffset? nextDate = items.Count > query.Limit ? items[^1].CreatedDateUtc : null;
        Guid? nextId = items.Count > query.Limit ? items[^1].Id : null;

        var cursor = nextDate is not null && nextId is not null
            ? Cursor.Encode(nextDate.Value, nextId.Value)
            : null;

        var hasMore = items.Count > query.Limit;

        var result = CursorResult<ClassDto>.Create(cursor, hasMore, finalItems);

        logger.LogInformation(
            "Get student classes succeeded. StudentId={StudentId}, ReturnedCount={ReturnedCount}, HasMore={HasMore}, NextCursor={NextCursor}",
            studentId, finalItems.Count, hasMore, cursor
        );

        return result;
    }
}
