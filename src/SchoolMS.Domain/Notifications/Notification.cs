using SchoolMS.Domain.Common;
using SchoolMS.Domain.Notifications.Enums;
using SchoolMS.Domain.Users;

namespace SchoolMS.Domain.Notifications;

public sealed class Notification : AuditableEntity
{
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public RecipientRole RecipientRole { get; private set; }
    public Guid? RecipientId { get; private set; }
    public bool IsRead { get; private set; } = false;
    public User? Recipient { get; private set; }
    private Notification(Guid id, string title, string message, RecipientRole recipientRole, Guid? recipientId) : base(id)
    {
        Title = title;
        Message = message;
        RecipientRole = recipientRole;
        RecipientId = recipientId;
    }


    public static Result<Notification> Create(Guid id, string title, string message, RecipientRole recipientRole, Guid? recipientId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return NotificationErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(message))
            return NotificationErrors.MessageRequired;

        return new Notification(id, title, message, recipientRole, recipientId);
    }
}

public static class NotificationErrors
{
    public static Error TitleRequired => Error.Validation("Notificatin.Title.Required", "Title is required.");
    public static Error MessageRequired => Error.Validation("Notificatin.Message.Required", "Message is required.");
}