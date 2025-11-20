using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Contracts.Courses;

public class UpdateCourseRequest
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid? DepartmentId { get; set; }
    public int Credits { get; set; }
}
