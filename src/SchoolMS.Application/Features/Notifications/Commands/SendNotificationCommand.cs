namespace SchoolMS.Application.Features.Notifications.Commands;

public class SendNotificationCommand : IRequest<Result<Success>>
{
    public bool IsClass { get; set; }
    public Guid? ClassId { get; set; }
    public Guid? StudentId { get; set; }
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
}
