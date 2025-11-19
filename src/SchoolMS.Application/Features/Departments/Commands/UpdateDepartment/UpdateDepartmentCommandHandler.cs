using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Domain.Departments;
using SchoolMS.Domain.Users;

namespace SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;

public class UpdateDepartmentCommandHandler(IAppDbContext context) : IRequestHandler<UpdateDepartmentCommand, Result<DepartmentDto>>
{
    public async Task<Result<DepartmentDto>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentQuery = context.Departments.AsQueryable();

        if (!command.HeadOfDepartmentId.HasValue)
        {
            departmentQuery = departmentQuery.Include(d => d.HeadOfDepartment);
        }

        var department = await departmentQuery.FirstOrDefaultAsync(dep => dep.Id == command.Id, cancellationToken);

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
            var headOfDepartment = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == command.HeadOfDepartmentId, cancellationToken);

            if (headOfDepartment is null)
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
            HeadOfDepartmentId = department.HeadOfDepartmentId,
            HeadOfDepartmentName = department.HeadOfDepartment.Name
        };
    }
}