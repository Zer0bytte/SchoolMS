using SchoolMS.Application.Features.Departments.Dtos;

namespace SchoolMS.Application.Features.Departments.Queries.GetDepartment;

public class GetDepartmentByIdQuery : IRequest<Result<DepartmentDto>>
{
    public Guid Id { get; set; }
}
