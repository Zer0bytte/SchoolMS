using SchoolMS.Application.Features.Identity;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.Users;
using System.Security.Claims;

namespace SchoolMS.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(User user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}