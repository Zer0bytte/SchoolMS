
namespace SchoolMS.Domain.Users;

public static class UserErrors
{
    public static Error NameRequired
       => Error.Validation("User.Name.Required", "Name is required.");

    public static Error EmailRequired
     => Error.Validation("User.Email.Required", "Email is required.");

    public static Error Password
     => Error.Validation("User.Password.Required", "Password is required.");

    public static Error InvalidEmail
     => Error.Validation("User.Email.Invalid", "Email format is invalid.");
}
