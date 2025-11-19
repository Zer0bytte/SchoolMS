namespace SchoolMS.Application.Features.Identity;

public class TokenResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime ExpiresOnUtc { get; set; }
}
