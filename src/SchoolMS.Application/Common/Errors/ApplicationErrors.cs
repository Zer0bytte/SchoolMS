namespace SchoolMS.Application.Common.Errors;

public static class ApplicationErrors
{
    public static Error EmailAlreadyExists =>
       Error.Validation(
           "User.Email.Exists",
           "User with this email already exists.");

    public static readonly Error ExpiredAccessTokenInvalid = Error.Conflict(
       code: "Auth.ExpiredAccessToken.Invalid",
       description: "Expired access token is not valid.");

    public static readonly Error UserIdClaimInvalid = Error.Conflict(
        code: "Auth.UserIdClaim.Invalid",
        description: "Invalid userId claim.");

    public static readonly Error RefreshTokenExpired = Error.Conflict(
      code: "Auth.RefreshToken.Expired",
      description: "Refresh token is invalid or has expired.");

    public static Error UserNotFound =>
   Error.NotFound(
       "User.NotFound", "User not found.");

    public static Error InvalidCredentials =>
         Error.Unauthorized(
              "Auth.InvalidCredentials",
              "The provided credentials are invalid.");

  
}
