using SchoolMS.Application.Features.Notifications.Dtos;

namespace SchoolMS.Application.Features.Notifications.Qureies.GetNotifications;

public class GetNotificationsQueryHandler(IAppDbContext context, IUser user) : IRequestHandler<GetNotificationsQuery, Result<List<NotificationDto>>>
{
    public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await context.Notifications.Where(n => n.RecipientId == Guid.Parse(user.Id)).Select(n => new NotificationDto
        {
            Id = n.Id,
            Message = n.Message,
            Title = n.Title,
            IsRead = n.IsRead
        }).ToListAsync(cancellationToken);

        return notifications;
    }
}