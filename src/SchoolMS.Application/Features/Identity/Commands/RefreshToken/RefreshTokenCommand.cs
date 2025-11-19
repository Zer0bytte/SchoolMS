namespace SchoolMS.Application.Features.Identity.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<TokenResponse>>
{
    public string RefreshToken { get; set; } = default!;
    public string ExpiredAccessToken { get; set; } = default!;
}