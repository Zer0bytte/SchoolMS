using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Identity.Dtos;

namespace SchoolMS.Application.Features.Identity.Queries.Login;

public class LoginQueryHandler(IAppDbContext context, ITokenProvider tokenProvider, IPasswordHasher passwordHasher) : IRequestHandler<LoginQuery, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null)
        {
            return ApplicationErrors.InvalidCredentials;
        }

        var passwordVerificationResult = passwordHasher.VerifyPassword(request.Password, user.Password);

        if (!passwordVerificationResult)
        {
            return ApplicationErrors.InvalidCredentials;
        }

        var userDto = new AppUserDto(user.Id, user.Email, user.Role);

        var tokenResponse = await tokenProvider.GenerateJwtTokenAsync(userDto, cancellationToken);

        if (tokenResponse.IsError) return tokenResponse.Errors;

        return tokenResponse;
    }
}
