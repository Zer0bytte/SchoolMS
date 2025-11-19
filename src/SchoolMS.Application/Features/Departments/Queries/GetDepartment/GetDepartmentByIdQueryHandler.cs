using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Departments.Queries.GetDepartment;

public class GetDepartmentByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .AsNoTracking()
            .Where(d => d.Id == request.Id)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                HeadOfDepartmentId = d.HeadOfDepartmentId,
                HeadOfDepartmentName = d.HeadOfDepartment.Name,
                CreatedDateUtc = d.CreatedDateUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
            return DepartmentErrors.NotFound;

        return department;
    }
}