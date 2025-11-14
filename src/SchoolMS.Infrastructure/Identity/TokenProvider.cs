using EedadyAbofam.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolMS.Application.Features.Identity;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EedadyAbofam.Infrastructure.Identity;

public class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<Result<TokenResponse>> GenerateJwtTokenAsync(User user, CancellationToken ct = default)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!)),
            ValidateIssuer = true,
            ValidIssuer = _configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["JwtSettings:Audience"],
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

    private async Task<Result<TokenResponse>> CreateAsync(User user, CancellationToken ct = default)
    {
        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");

        string issuer = jwtSettings["Issuer"]!;
        string audience = jwtSettings["Audience"]!;
        string key = jwtSettings["Secret"]!;

        DateTime expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));

        List<Claim> claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
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



        return new TokenResponse
        {
            AccessToken = tokenHandler.WriteToken(securityToken),
            ExpiresOnUtc = expires
        };
    }

}