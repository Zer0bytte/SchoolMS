namespace SchoolMS.Contracts.Notifications;

public class SendNotificationRequest
{
    public bool IsClass { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? StudentId { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
}
