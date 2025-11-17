using SchoolMS.Domain.Common.Results;

namespace SchoolMS.Application.Common.Errors;

public static class ApplicationErrors
{
    public static Error EmailAlreadyExists =>
       Error.Validation(
           "User.Email.Exists",
           "User with this email already exists.");
}
