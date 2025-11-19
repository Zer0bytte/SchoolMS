namespace SchoolMS.Contracts.Departments;

public class UpdateDepartmentRequest
{
    public string?Name { get; set; } 
    public string? Description { get; set; }
    public Guid? HeadOfDepartmentId { get; set; }
}
