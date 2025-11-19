using SchoolMS.Application.Features.Identity.Dtos;

namespace SchoolMS.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string? policyName);
    Task<Result<AppUserDto>> AuthenticateAsync(string email, string password);
    Task<Result<AppUserDto>> GetUserByIdAsync(string userId);
    Task<Result<string>> GetUserIdByUsername(string username);
    Task<string?> GetUserNameAsync(string userId);
    public Task<Result<Created>> RegisterServant(string username, string name);
    public Task<Result<Updated>> AssignPassword(string userName, string password);
    public Task<Result<Updated>> ChangePasswordAsync(string oldPassword, string newPassword, string newPasswordConfirmed);
    public Task<Result<Success>> BanUserAsync(string userId);
    public Task<Result<Success>> UnbanUser(string userId);
}
