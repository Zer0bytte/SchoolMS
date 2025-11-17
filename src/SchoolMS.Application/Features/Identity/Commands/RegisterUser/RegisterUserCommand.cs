using MediatR;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Identity.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<Result<TokenResponse>>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Role Role { get; set; }
}
