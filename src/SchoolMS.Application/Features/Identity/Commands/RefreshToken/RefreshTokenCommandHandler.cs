using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Identity.Dtos;
using System.Security.Claims;

namespace SchoolMS.Application.Features.Identity.Commands.RefreshToken;

public class RefreshTokenCommandHandler(ILogger<RefreshTokenCommandHandler> logger, IAppDbContext context, ITokenProvider tokenProvider)
    : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
{


    public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var principal = tokenProvider.GetPrincipalFromExpiredToken(request.ExpiredAccessToken);

        if (principal is null)
        {
            logger.LogError("Expired access token is not valid");

            return ApplicationErrors.ExpiredAccessTokenInvalid;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            logger.LogError("Invalid userId claim");

            return ApplicationErrors.UserIdClaimInvalid;
        }

        var getUserResult = await context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), ct);

        if (getUserResult is null)
        {
            return ApplicationErrors.UserNotFound;
        }

        var userDto = new AppUserDto
        (
             getUserResult.Id,
             getUserResult.Email,
             getUserResult.Role
             );

        var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == request.RefreshToken && r.UserId == Guid.Parse(userId), ct);

        if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            logger.LogError("Refresh token has expired");

            return ApplicationErrors.RefreshTokenExpired;
        }

        var generateTokenResult = await tokenProvider.GenerateJwtTokenAsync(userDto, ct);

        if (generateTokenResult.IsError)
        {
            logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);

            return generateTokenResult.Errors;
        }

        return generateTokenResult.Value;
    }
}