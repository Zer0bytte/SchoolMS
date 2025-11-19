using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMS.Application.Features.Identity.Queries.Login;

public class LoginQuery : IRequest<Result<TokenResponse>>
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
