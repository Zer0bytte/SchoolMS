using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Notifications.Commands;
using SchoolMS.Contracts.Notifications;

namespace SchoolMS.Api.Endpoints;

public static class TeacherNotificationEndpoints
{
    public static void MapTeacherNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/teacher/notifications").RequireAuthorization("Teacher"); 
        group.MapPost("", SendNotification);
    }

    private static async Task<IResult> SendNotification(SendNotificationRequest request, ISender sender)
    {
        var command = new SendNotificationCommand
        {
            ClassId = request.ClassId,
            IsClass = request.IsClass,
            Message = request.Message,
            StudentId = request.StudentId,
            Title = request.Title,
        };

        var result = await sender.Send(command);

        return result.Match
        (result => Results.NoContent(),
        error => error.ToProblem());
    }
}
