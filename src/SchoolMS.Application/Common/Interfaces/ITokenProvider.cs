using SchoolMS.Application.Features.Identity;
using SchoolMS.Application.Features.Identity.Dtos;
using System.Security.Claims;

namespace SchoolMS.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}