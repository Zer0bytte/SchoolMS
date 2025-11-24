namespace SchoolMS.Application.Features.Notifications.Dtos;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get;  set; } = default!;
    public string Message { get;  set; } = default!;
    public bool IsRead { get;  set; } = false;

}
