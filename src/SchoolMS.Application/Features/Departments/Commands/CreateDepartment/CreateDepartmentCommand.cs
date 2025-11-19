using SchoolMS.Application.Features.Departments.Dtos;

namespace SchoolMS.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommand : IRequest<Result<DepartmentDto>>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid HeadOfDepartmentId { get; set; }
}
