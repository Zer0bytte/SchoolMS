using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Queries.Teacher.GetTeacherClasses;

public class GetTeacherClassesQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetTeacherClassesQueryHandler> logger
) : IRequestHandler<GetTeacherClassesQuery, Result<CursorResult<ClassDto>>>
{
    public async Task<Result<CursorResult<ClassDto>>> Handle(GetTeacherClassesQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Get teacher classes failed: user id missing. Cursor={Cursor}, Limit={Limit}",
                query.Cursor, query.Limit
            );
            return ApplicationErrors.UserNotFound;
        }

        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get teacher classes started. TeacherId={TeacherId}, Cursor={Cursor}, Limit={Limit}",
            teacherId, query.Cursor, query.Limit
        );

        var dbQuery = context.Classes
            .Where(c => c.TeacherId == teacherId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            logger.LogDebug(
                "Decoding cursor for teacher classes. TeacherId={TeacherId}, Cursor={Cursor}",
                teacherId, query.Cursor
            );

            var decodedCursor = Cursor.Decode(query.Cursor);
            if (decodedCursor is null)
            {
                logger.LogWarning(
                    "Get teacher classes failed: invalid cursor. TeacherId={TeacherId}, Cursor={Cursor}",
                    teacherId, query.Cursor
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
            "Get teacher classes succeeded. TeacherId={TeacherId}, ReturnedCount={ReturnedCount}, HasMore={HasMore}, NextCursor={NextCursor}",
            teacherId, finalItems.Count, hasMore, cursor
        );

        return result;
    }
}
