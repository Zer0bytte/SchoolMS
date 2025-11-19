using SchoolMS.Application.Features.Departments.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Features.Departments.Queries.GetDepartment;

public class GetDepartmentByIdQuery : IRequest<Result<DepartmentDto>>
{
    public Guid Id { get; set; }
}
