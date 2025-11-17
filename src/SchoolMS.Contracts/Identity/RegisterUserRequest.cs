using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Contracts.Identity;

public class RegisterUserRequest
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Role Role { get; set; }
}
