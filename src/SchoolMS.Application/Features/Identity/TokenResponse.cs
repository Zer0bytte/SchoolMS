namespace SchoolMS.Application.Features.Identity;

public class TokenResponse
{
    public string? AccessToken { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
}
