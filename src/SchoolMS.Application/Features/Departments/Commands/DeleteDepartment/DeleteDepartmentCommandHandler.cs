using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Departments.Commands.RemoveDepartment;

public class DeleteDepartmentCommandHandler(IAppDbContext context) : IRequestHandler<DeleteDepartmentCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await context.Departments.FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (department is null)
            return DepartmentErrors.NotFound;

        context.Departments.Remove(department);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}