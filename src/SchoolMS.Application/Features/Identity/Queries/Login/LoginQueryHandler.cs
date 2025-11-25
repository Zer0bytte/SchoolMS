using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Identity.Dtos;

namespace SchoolMS.Application.Features.Identity.Queries.Login;

public class LoginQueryHandler(
    IAppDbContext context,
    ITokenProvider tokenProvider,
    IPasswordHasher passwordHasher,
    ILogger<LoginQueryHandler> logger
) : IRequestHandler<LoginQuery, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Login attempt started. Email={Email}",
            request.Email
        );

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            logger.LogWarning(
                "Login failed: user not found. Email={Email}",
                request.Email
            );
            return ApplicationErrors.InvalidCredentials;
        }

        var passwordVerificationResult = passwordHasher.VerifyPassword(request.Password, user.Password);

        if (!passwordVerificationResult)
        {
            logger.LogWarning(
                "Login failed: invalid password. Email={Email}, UserId={UserId}",
                request.Email, user.Id
            );
            return ApplicationErrors.InvalidCredentials;
        }

        var userDto = new AppUserDto(user.Id, user.Email, user.Role);

        logger.LogDebug(
            "Generating JWT token. UserId={UserId}, Email={Email}, Role={Role}",
            user.Id, user.Email, user.Role
        );

        var tokenResponse = await tokenProvider.GenerateJwtTokenAsync(userDto, cancellationToken);

        if (tokenResponse.IsError)
        {
            logger.LogWarning(
                "Login failed: token generation error. UserId={UserId}, Email={Email}, Errors={Errors}",
                user.Id, user.Email, tokenResponse.Errors
            );
            return tokenResponse.Errors;
        }

        logger.LogInformation(
            "Login succeeded. UserId={UserId}, Email={Email}, Role={Role}",
            user.Id, user.Email, user.Role
        );

        return tokenResponse;
    }
}
