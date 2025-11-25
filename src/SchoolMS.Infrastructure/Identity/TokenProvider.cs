using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Identity;
using SchoolMS.Application.Features.Identity.Dtos;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.RefreshTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SchoolMS.Infrastructure.Identity;

public class TokenProvider(IConfiguration configuration, IAppDbContext context) : ITokenProvider
{
    public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default)
    {
        Result<TokenResponse> tokenResult = await CreateAsync(user, ct);

        if (tokenResult.IsError)
        {
            return tokenResult.Errors;
        }

        return tokenResult.Value;
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!)),
            ValidateIssuer = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token.");
        }

        return principal;
    }

    private async Task<Result<TokenResponse>> CreateAsync(AppUserDto user, CancellationToken ct = default)
    {
        IConfigurationSection jwtSettings = configuration.GetSection("JwtSettings");

        string issuer = jwtSettings["Issuer"]!;
        string audience = jwtSettings["Audience"]!;
        string key = jwtSettings["Secret"]!;

        DateTime expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));
        int refreshTokenExpirationInDays = int.Parse(jwtSettings["RefreshTokenExpirationInDays"]!);
        List<Claim> claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256Signature),
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken securityToken = tokenHandler.CreateToken(descriptor);

        await context.RefreshTokens
             .Where(rt => rt.UserId == user.UserId)
             .ExecuteDeleteAsync(ct);

        Result<RefreshToken> refreshTokenResult = RefreshToken.Create(
            Guid.NewGuid(),
            GenerateRefreshToken(),
            user.UserId,
            DateTime.UtcNow.AddDays(refreshTokenExpirationInDays),
            DateTimeOffset.UtcNow);

        if (refreshTokenResult.IsError)
        {
            return refreshTokenResult.Errors;
        }

        RefreshToken refreshToken = refreshTokenResult.Value;

        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync(ct);

        return new TokenResponse
        {
            UserId = user.UserId,
            AccessToken = tokenHandler.WriteToken(securityToken),
            RefreshToken = refreshToken.Token,
            Role = user.Role.ToString(),
            ExpiresOnUtc = expires
        };
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

}