namespace SchoolMS.Application.Common.Interfaces;

public interface IUser
{
    string? Id { get; }
    string? ManagedGroupId { get; }
    string? Role { get; }

}
