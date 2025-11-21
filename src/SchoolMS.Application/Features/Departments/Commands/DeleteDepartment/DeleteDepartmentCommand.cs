namespace SchoolMS.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommand : IRequest<Result<Success>>
{
    public Guid DepartmentId { get; set; }
}
