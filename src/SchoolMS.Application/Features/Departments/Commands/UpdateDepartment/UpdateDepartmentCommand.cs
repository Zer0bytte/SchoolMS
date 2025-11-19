using SchoolMS.Application.Features.Departments.Dtos;

namespace SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommand : IRequest<Result<DepartmentDto>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = default!;
    public string? Description { get; set; } = default!;
    public Guid? HeadOfDepartmentId { get; set; }
}
