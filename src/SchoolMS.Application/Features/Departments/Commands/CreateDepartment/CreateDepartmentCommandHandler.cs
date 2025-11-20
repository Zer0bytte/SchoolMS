using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users.Enums;

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

        var headOfDepartment = await context.Users.FirstOrDefaultAsync(u => u.Id == command.HeadOfDepartmentId, cancellationToken);

        if (headOfDepartment is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        if (headOfDepartment.Role != Role.Teacher)
        {
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
            HeadOfDepartmentId = headOfDepartment.Id,
            HeadOfDepartmentName = headOfDepartment.Name,
            CreatedDateUtc = department.CreatedDateUtc
        };
    }
}