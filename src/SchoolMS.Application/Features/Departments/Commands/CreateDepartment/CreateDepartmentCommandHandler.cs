using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentCommandHandler(IAppDbContext context) : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentNameExist = await context.Departments.AnyAsync(d => d.Name == command.Name, cancellationToken);

        if (departmentNameExist)
        {
            return DepartmentErrors.DublicateName;
        }

        var headOfDepartmentExists = await context.Users.AnyAsync(u => u.Id == command.HeadOfDepartmentId, cancellationToken);

        if (!headOfDepartmentExists)
        {
            return ApplicationErrors.UserNotFound;
        }


        var departmentResult = Department.Create(
            Guid.CreateVersion7(),
            command.Name,
            command.Description,
            command.HeadOfDepartmentId
        );

        if (departmentResult.IsError)
        {
            return departmentResult.Errors;
        }

        var department = departmentResult.Value;
        context.Departments.Add(department);

        await context.SaveChangesAsync(cancellationToken);

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            HeadOfDepartment = new HeadOfDepartmentDto
            {
                Id = department.HeadOfDepartmentId,
            }
        };
    }
}