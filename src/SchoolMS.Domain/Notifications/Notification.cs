using SchoolMS.Domain.Common;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Domain.Notifications;

public class Notification : Entity
{
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Role RecipientRole { get; set; }
    public Guid? RecipientId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public bool IsRead { get; set; } = false;
    public User? Recipient { get; set; }
}