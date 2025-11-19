using SchoolMS.Domain.Common;

namespace SchoolMS.Domain.RefreshTokens;

public sealed class RefreshToken : AuditableEntity
{
    public string Token { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public DateTimeOffset ExpiresOnUtc { get; private set; }

    private RefreshToken()
    { }

    private RefreshToken(Guid id, string token, Guid userId, DateTimeOffset expiresOnUtc)
        : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresOnUtc = expiresOnUtc;
    }

    public static Result<RefreshToken> Create(Guid id, string token, Guid userId, DateTimeOffset expiresOnUtc, DateTimeOffset timeNow)
    {
        if (id == Guid.Empty)
        {
            return RefreshTokenErrors.IdRequired;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return RefreshTokenErrors.TokenRequired;
        }

        if (expiresOnUtc <= timeNow)
        {
            return RefreshTokenErrors.ExpiryInvalid;
        }

        return new RefreshToken(id, token, userId, expiresOnUtc);
    }
}