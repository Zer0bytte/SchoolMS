namespace SchoolMS.Contracts.Departments;

public class CreateDepartmentRequest
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Guid HeadOfDepartmentId { get; set; }
}
