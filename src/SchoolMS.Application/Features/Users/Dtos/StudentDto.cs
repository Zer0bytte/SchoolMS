namespace SchoolMS.Application.Features.Users.Dtos;

public class StudentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTimeOffset CreatedDateUtc { get; set; }
}
