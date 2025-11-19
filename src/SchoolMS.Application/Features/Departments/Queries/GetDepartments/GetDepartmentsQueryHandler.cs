using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Departments.Dtos;

namespace SchoolMS.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsQueryHandler(IAppDbContext context) : IRequestHandler<GetDepartmentsQuery, Result<DepartmentResult>>
{
    public async Task<Result<DepartmentResult>> Handle(GetDepartmentsQuery query, CancellationToken cancellationToken)
    {
        var dbQuery = context.Departments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.DepartmentName))
        {
            dbQuery = dbQuery.Where(d => d.Name.Contains(query.DepartmentName));
        }

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
            .OrderByDescending(d => d.CreatedDateUtc)
            .ThenByDescending(d => d.Id)
            .Take(query.Limit + 1)
            .Select(d=> new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                CreatedDateUtc = d.CreatedDateUtc,
                HeadOfDepartment = new HeadOfDepartmentDto
                {
                    Id = d.HeadOfDepartment.Id,
                    Name = d.HeadOfDepartment.Name
                },
            })
            .ToListAsync(cancellationToken);

        var finalItems = items.Take(query.Limit).ToList();

        DateTimeOffset? nextDate = items.Count > query.Limit ? items[^1].CreatedDateUtc : null;
        Guid? nextId = items.Count > query.Limit ? items[^1].Id : null;

        return new DepartmentResult
        {
            Items = finalItems,
            Cursor = nextDate is not null && nextId is not null
                ? Cursor.Encode(nextDate.Value, nextId.Value) : null,
            HasMore = items.Count > query.Limit
        };

    }
}