using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Courses.Dtos;

namespace SchoolMS.Application.Features.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler(
    IAppDbContext context,
    HybridCache cache,
    ILogger<GetCoursesQueryHandler> logger
) : IRequestHandler<GetCoursesQuery, Result<CursorResult<CourseDto>>>
{
    public async Task<Result<CursorResult<CourseDto>>> Handle(GetCoursesQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(query);

        logger.LogInformation(
            "Get courses started. DepartmentId={DepartmentId}, SearchTerm={SearchTerm}, Cursor={Cursor}, Limit={Limit}, CacheKey={CacheKey}",
            query.DepartmentId, query.SearchTerm, query.Cursor, query.Limit, cacheKey
        );

        return await cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var dbQuery = context.Courses.AsQueryable();

                if (query.DepartmentId.HasValue)
                {
                    dbQuery = dbQuery.Where(c => c.DepartmentId == query.DepartmentId.Value);
                }

                if (!string.IsNullOrWhiteSpace(query.Cursor))
                {
                    var decodedCursor = Cursor.Decode(query.Cursor);
                    if (decodedCursor is null)
                    {
                        logger.LogWarning(
                            "Get courses failed: invalid cursor. Cursor={Cursor}",
                            query.Cursor
                        );

                        // DON'T cache invalid cursor responses
                        return Error.Failure("InvalidCursor", "The provided cursor is invalid.");
                    }

                    dbQuery = dbQuery.Where(d => d.CreatedDateUtc < decodedCursor.Date ||
                                                 d.CreatedDateUtc == decodedCursor.Date &&
                                                 d.Id <= decodedCursor.LastId);
                }

                if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                {
                    var searchTerm = query.SearchTerm.ToLower();
                    dbQuery = dbQuery.Where(c =>
                        c.Name.ToLower().Contains(searchTerm) ||
                        c.Code.ToLower().Contains(searchTerm) ||
                        c.Description.ToLower().Contains(searchTerm));
                }

                var items = await dbQuery
                    .AsNoTracking()
                    .OrderByDescending(d => d.CreatedDateUtc)
                    .ThenByDescending(d => d.Id)
                    .Take(query.Limit + 1)
                    .Select(c => new CourseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Code = c.Code,
                        Description = c.Description,
                        DepartmentId = c.DepartmentId,
                        DepartmentName = c.Department.Name,
                        Credits = c.Credits,
                        CreatedDateUTC = c.CreatedDateUtc,
                    })
                    .ToListAsync(ct);

                var finalItems = items.Take(query.Limit).ToList();

                DateTimeOffset? nextDate = items.Count > query.Limit ? items[^1].CreatedDateUTC : null;
                Guid? nextId = items.Count > query.Limit ? items[^1].Id : null;

                var cursor = nextDate is not null && nextId is not null
                    ? Cursor.Encode(nextDate.Value, nextId.Value)
                    : null;

                var hasMore = items.Count > query.Limit;

                var result = CursorResult<CourseDto>.Create(cursor, hasMore, finalItems);

                logger.LogInformation(
                    "Get courses succeeded (computed). ReturnedCount={ReturnedCount}, HasMore={HasMore}, NextCursor={NextCursor}",
                    finalItems.Count, hasMore, cursor
                );

                return result;
            },
            tags: new[] { "courses" },
            cancellationToken: cancellationToken
        );
    }

    private static string BuildCacheKey(GetCoursesQuery query)
    {
        var dept = query.DepartmentId?.ToString() ?? "all";
        var search = query.SearchTerm?.Trim().ToLowerInvariant() ?? "";
        var cursor = query.Cursor?.Trim() ?? "";
        var limit = query.Limit;

        return $"courses:v1:dept:{dept}:search:{search}:cursor:{cursor}:limit:{limit}";
    }
}
