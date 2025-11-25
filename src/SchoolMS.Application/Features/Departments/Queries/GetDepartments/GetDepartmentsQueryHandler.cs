using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsQueryHandler(
    IAppDbContext context,
    IUser user,
    ILogger<GetDepartmentsQueryHandler> logger
) : IRequestHandler<GetDepartmentsQuery, Result<CursorResult<DepartmentDto>>>
{
    public async Task<Result<CursorResult<DepartmentDto>>> Handle(GetDepartmentsQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Get departments failed: user id missing. Cursor={Cursor}, Limit={Limit}, DepartmentName={DepartmentName}, Role={Role}",
                query.Cursor, query.Limit, query.DepartmentName, user.Role
            );
            return ApplicationErrors.UserNotFound;
        }

        var userId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Get departments started. UserId={UserId}, Role={Role}, Cursor={Cursor}, Limit={Limit}, DepartmentName={DepartmentName}",
            userId, user.Role, query.Cursor, query.Limit, query.DepartmentName
        );

        var dbQuery = context.Departments.AsQueryable();

        if (user.Role == Role.Teacher.ToString())
        {
            logger.LogDebug(
                "Filtering departments by head of department. UserId={UserId}",
                userId
            );

            dbQuery = dbQuery.Where(d => d.HeadOfDepartmentId == userId);
        }

        if (!string.IsNullOrWhiteSpace(query.DepartmentName))
        {
            logger.LogDebug(
                "Filtering departments by name. UserId={UserId}, DepartmentName={DepartmentName}",
                userId, query.DepartmentName
            );

            dbQuery = dbQuery.Where(d => d.Name.Contains(query.DepartmentName));
        }

        if (!string.IsNullOrWhiteSpace(query.Cursor))
        {
            logger.LogDebug(
                "Decoding cursor for departments. UserId={UserId}, Cursor={Cursor}",
                userId, query.Cursor
            );

            var decodedCursor = Cursor.Decode(query.Cursor);
            if (decodedCursor is null)
            {
                logger.LogWarning(
                    "Get departments failed: invalid cursor. UserId={UserId}, Cursor={Cursor}",
                    userId, query.Cursor
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
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                CreatedDateUtc = d.CreatedDateUtc,
                HeadOfDepartmentId = d.HeadOfDepartmentId,
                HeadOfDepartmentName = d.HeadOfDepartment.Name
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
            "Get departments succeeded. UserId={UserId}, ReturnedCount={ReturnedCount}, HasMore={HasMore}, NextCursor={NextCursor}",
            userId, finalItems.Count, hasMore, cursor
        );

        return CursorResult<DepartmentDto>.Create(cursor, hasMore, finalItems);
    }
}
