namespace SchoolMS.Application.Features.Departments.Commands.RemoveDepartment;

public class DeleteDepartmentCommand : IRequest<Result<Deleted>>
{
    public Guid DepartmentId { get; set; }
}
