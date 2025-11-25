using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler(
    IAppDbContext context,
    ILogger<CreateDepartmentCommandHandler> logger
) : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Create department started. Name={Name}, HeadOfDepartmentId={HeadOfDepartmentId}",
            command.Name, command.HeadOfDepartmentId
        );

        var departmentNameExist = await context.Departments
            .AnyAsync(d => d.Name == command.Name, cancellationToken);

        if (departmentNameExist)
        {
            logger.LogWarning(
                "Create department failed: duplicate department name. Name={Name}",
                command.Name
            );
            return DepartmentErrors.DublicateName;
        }

        var headOfDepartment = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.HeadOfDepartmentId, cancellationToken);

        if (headOfDepartment is null)
        {
            logger.LogWarning(
                "Create department failed: head of department user not found. HeadOfDepartmentId={HeadOfDepartmentId}, Name={Name}",
                command.HeadOfDepartmentId, command.Name
            );
            return ApplicationErrors.UserNotFound;
        }

        if (headOfDepartment.Role != Role.Teacher)
        {
            logger.LogWarning(
                "Create department failed: head of department must be a teacher. HeadOfDepartmentId={HeadOfDepartmentId}, ActualRole={Role}",
                command.HeadOfDepartmentId, headOfDepartment.Role
            );
            return ApplicationErrors.HeadOfDepartmentShouldBeTeacher;
        }

        var departmentResult = Department.Create(
            Guid.CreateVersion7(),
            command.Name,
            command.Description,
            command.HeadOfDepartmentId
        );

        if (departmentResult.IsError)
        {
            logger.LogWarning(
                "Create department failed: domain validation errors. Name={Name}, HeadOfDepartmentId={HeadOfDepartmentId}, Errors={Errors}",
                command.Name, command.HeadOfDepartmentId, departmentResult.Errors
            );
            return departmentResult.Errors;
        }

        var department = departmentResult.Value;
        context.Departments.Add(department);

        await context.SaveChangesAsync(cancellationToken);

        var dto = new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            HeadOfDepartmentId = headOfDepartment.Id,
            HeadOfDepartmentName = headOfDepartment.Name,
            CreatedDateUtc = department.CreatedDateUtc
        };

        logger.LogInformation(
            "Create department succeeded. DepartmentId={DepartmentId}, Name={Name}, HeadOfDepartmentId={HeadOfDepartmentId}",
            dto.Id, dto.Name, dto.HeadOfDepartmentId
        );

        return dto;
    }
}
