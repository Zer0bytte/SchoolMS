namespace SchoolMS.Domain.RefreshTokens;

public static class RefreshTokenErrors
{
    public static readonly Error IdRequired =
        Error.Validation("RefreshToken.Id.Required", "Refresh token ID is required.");

    public static readonly Error TokenRequired =
        Error.Validation("RefreshToken.Id.Required", "Token value is required.");


    public static readonly Error ExpiryInvalid =
        Error.Validation("RefreshToken.Expiry.Invalid", "Expiry must be in the future.");
}