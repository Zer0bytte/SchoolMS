using MediatR;
using SchoolMS.Domain.Common.Results;

namespace SchoolMS.Application.Features.Identity.Commands.RegisterAdmin;

public class RegisterAdminCommand : IRequest<Result<Created>>
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
