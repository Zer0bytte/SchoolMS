using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Departments.Queries.GetDepartment;

public class GetDepartmentByIdQueryHandler(
    IAppDbContext context,
    ILogger<GetDepartmentByIdQueryHandler> logger
) : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Get department by id started. DepartmentId={DepartmentId}",
            request.Id
        );

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
        {
            logger.LogWarning(
                "Get department by id failed: department not found. DepartmentId={DepartmentId}",
                request.Id
            );
            return DepartmentErrors.NotFound;
        }

        logger.LogInformation(
            "Get department by id succeeded. DepartmentId={DepartmentId}, Name={Name}, HeadOfDepartmentId={HeadOfDepartmentId}",
            department.Id, department.Name, department.HeadOfDepartmentId
        );

        return department;
    }
}
