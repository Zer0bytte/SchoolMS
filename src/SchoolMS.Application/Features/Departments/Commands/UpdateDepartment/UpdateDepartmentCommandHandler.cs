using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Departments;

namespace SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler(IAppDbContext context) : IRequestHandler<UpdateDepartmentCommand, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var department = await context.Departments.FirstOrDefaultAsync(dep => dep.Id == command.Id, cancellationToken);
        if (department is null)
        {
            return DepartmentErrors.NotFound;
        }

        var departmentNameExist = await context.Departments
            .AnyAsync(d => d.Name == command.Name && d.Id != command.Id, cancellationToken);

        if (departmentNameExist)
        {
            return DepartmentErrors.DublicateName;
        }

        if (command.HeadOfDepartmentId.HasValue)
        {
            var headOfDepartmentExists = await context.Users.AnyAsync(u => u.Id == command.HeadOfDepartmentId, cancellationToken);

            if (!headOfDepartmentExists)
            {
                return ApplicationErrors.UserNotFound;
            }

        }

        department.Update(
           command.Name,
           command.Description,
           command.HeadOfDepartmentId
       );


        await context.SaveChangesAsync(cancellationToken);
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            HeadOfDepartment = new HeadOfDepartmentDto
            {
                Id = department.HeadOfDepartmentId
            }
        };
    }
}