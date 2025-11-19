using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Departments.Commands.RemoveDepartment;

public class DeleteDepartmentCommandHandler(IAppDbContext context) : IRequestHandler<DeleteDepartmentCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await context.Departments.FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (department is null)
            return DepartmentErrors.NotFound;

        context.Departments.Remove(department);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}