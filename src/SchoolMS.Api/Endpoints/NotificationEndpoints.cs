using MediatR;
using Microsoft.AspNetCore.Http;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Notifications.Commands.SendNotification;
using SchoolMS.Application.Features.Notifications.Dtos;
using SchoolMS.Application.Features.Notifications.Qureies.GetNotifications;
using SchoolMS.Contracts.Notifications;

namespace SchoolMS.Api.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapPost("/api/teacher/notifications", SendNotification)
            .RequireAuthorization("Teacher")
            .WithName("SendNotification")
            .WithTags("Notifications")
            .WithSummary("Send a notification to a student or a class")
            .WithDescription("""
                Allows an authenticated teacher to send a notification either 
                to an individual student or an entire class.  
                
                - If `IsClass = true`, the notification will be sent to all students in the specified class.  
                - If `IsClass = false`, the notification will be sent only to the specified student.  
                """)
            .Accepts<SendNotificationRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);


        app.MapGet("/api/student/notifications", GetNotifications)
              .RequireAuthorization("Student")
              .WithName("GetNotifications")
              .WithSummary("Retrieve student notifications")
              .WithDescription("Retrieves all notifications for the authenticated student")
              .WithTags("Notifications")
              .Produces<List<NotificationDto>>(StatusCodes.Status200OK)
              .ProducesValidationProblem()
              .Produces(StatusCodes.Status401Unauthorized)
              .Produces(StatusCodes.Status403Forbidden)
              .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetNotifications(ISender sender)
    {
        GetNotificationsQuery query = new GetNotificationsQuery();

        Domain.Common.Results.Result<List<NotificationDto>> result = await sender.Send(query);

        return result.Match(
                    result => Results.Ok(result),
                    error => error.ToProblem());
    }

    private static async Task<IResult> SendNotification(SendNotificationRequest request, ISender sender)
    {
        SendNotificationCommand command = new SendNotificationCommand
        {
            ClassId = request.ClassId,
            IsClass = request.IsClass,
            Message = request.Message,
            StudentId = request.StudentId,
            Title = request.Title,
        };

        Domain.Common.Results.Result<Domain.Common.Results.Success> result = await sender.Send(command);

        return result.Match
        (result => Results.NoContent(),
        error => error.ToProblem());
    }
}
