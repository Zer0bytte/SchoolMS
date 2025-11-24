using SchoolMS.Application.Features.Notifications.Dtos;

namespace SchoolMS.Application.Features.Notifications.Qureies.GetNotifications;

public class GetNotificationsQuery : IRequest<Result<List<NotificationDto>>>
{
}
