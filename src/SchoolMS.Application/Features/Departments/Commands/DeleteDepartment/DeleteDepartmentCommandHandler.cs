using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandHandler(
    IAppDbContext context,
    ILogger<DeleteDepartmentCommandHandler> logger
) : IRequestHandler<DeleteDepartmentCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Delete department started. DepartmentId={DepartmentId}",
            request.DepartmentId
        );

        var department = await context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (department is null)
        {
            logger.LogWarning(
                "Delete department failed: department not found. DepartmentId={DepartmentId}",
                request.DepartmentId
            );
            return DepartmentErrors.NotFound;
        }

        context.Departments.Remove(department);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Delete department succeeded. DepartmentId={DepartmentId}",
            request.DepartmentId
        );

        return Result.Success;
    }
}
