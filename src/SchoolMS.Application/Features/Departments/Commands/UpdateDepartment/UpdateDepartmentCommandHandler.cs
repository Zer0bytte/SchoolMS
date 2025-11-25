using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler(
    IAppDbContext context,
    ILogger<UpdateDepartmentCommandHandler> logger,
    HybridCache cache
) : IRequestHandler<UpdateDepartmentCommand, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Update department started. DepartmentId={DepartmentId}, Name={Name}, HeadOfDepartmentId={HeadOfDepartmentId}",
            command.Id, command.Name, command.HeadOfDepartmentId
        );

        var departmentQuery = context.Departments.AsQueryable();

        if (!command.HeadOfDepartmentId.HasValue)
        {
            logger.LogDebug(
                "Including HeadOfDepartment navigation because HeadOfDepartmentId not provided. DepartmentId={DepartmentId}",
                command.Id
            );
            departmentQuery = departmentQuery.Include(d => d.HeadOfDepartment);
        }

        var department = await departmentQuery
            .FirstOrDefaultAsync(dep => dep.Id == command.Id, cancellationToken);

        if (department is null)
        {
            logger.LogWarning(
                "Update department failed: department not found. DepartmentId={DepartmentId}",
                command.Id
            );
            return DepartmentErrors.NotFound;
        }

        var departmentNameExist = await context.Departments
            .AnyAsync(d => d.Name == command.Name && d.Id != command.Id, cancellationToken);

        if (departmentNameExist)
        {
            logger.LogWarning(
                "Update department failed: duplicate department name. DepartmentId={DepartmentId}, Name={Name}",
                command.Id, command.Name
            );
            return DepartmentErrors.DublicateName;
        }

        if (command.HeadOfDepartmentId.HasValue)
        {
            logger.LogDebug(
                "Validating new HeadOfDepartment. DepartmentId={DepartmentId}, HeadOfDepartmentId={HeadOfDepartmentId}",
                command.Id, command.HeadOfDepartmentId
            );

            var headOfDepartment = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == command.HeadOfDepartmentId, cancellationToken);

            if (headOfDepartment is null)
            {
                logger.LogWarning(
                    "Update department failed: head of department user not found. DepartmentId={DepartmentId}, HeadOfDepartmentId={HeadOfDepartmentId}",
                    command.Id, command.HeadOfDepartmentId
                );
                return ApplicationErrors.UserNotFound;
            }

            if (headOfDepartment.Role != Role.Teacher)
            {
                logger.LogWarning(
                    "Update department failed: head of department must be a teacher. DepartmentId={DepartmentId}, HeadOfDepartmentId={HeadOfDepartmentId}, ActualRole={Role}",
                    command.Id, command.HeadOfDepartmentId, headOfDepartment.Role
                );
                return ApplicationErrors.HeadOfDepartmentShouldBeTeacher;
            }
        }

        department.Update(
            command.Name,
            command.Description,
            command.HeadOfDepartmentId
        );

        await context.SaveChangesAsync(cancellationToken);
        await cache.RemoveByTagAsync("departments");

        var dto = new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            HeadOfDepartmentId = department.HeadOfDepartmentId,
            HeadOfDepartmentName = department.HeadOfDepartment.Name
        };

        logger.LogInformation(
            "Update department succeeded. DepartmentId={DepartmentId}, Name={Name}, HeadOfDepartmentId={HeadOfDepartmentId}",
            dto.Id, dto.Name, dto.HeadOfDepartmentId
        );

        return dto;
    }
}
