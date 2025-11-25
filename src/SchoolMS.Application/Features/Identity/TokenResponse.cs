namespace SchoolMS.Application.Features.Identity;

public class TokenResponse
{
    public Guid UserId { get; set; }
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime ExpiresOnUtc { get; set; }
}
