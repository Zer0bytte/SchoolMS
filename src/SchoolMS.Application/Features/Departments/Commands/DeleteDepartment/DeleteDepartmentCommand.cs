namespace SchoolMS.Application.Features.Departments.Commands.RemoveDepartment;

public class DeleteDepartmentCommand : IRequest<Result<Success>>
{
    public Guid DepartmentId { get; set; }
}
