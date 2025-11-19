namespace SchoolMS.Contracts.Identity;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = default!;
    public string ExpiredAccessToken { get; set; } = default!;
}
