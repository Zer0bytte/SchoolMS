using Asp.Versioning.Builder;
using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Application.Features.Classes.Queries.Student.GetStudentAttendance;
using SchoolMS.Application.Features.Classes.Queries.Student.GetStudentClasses;
using SchoolMS.Contracts.Classes;

namespace SchoolMS.Api.Endpoints;

public static class StudentClassesEndpoints
{
    public static void MapStudentClassesEndpoints(this IEndpointRouteBuilder app, ApiVersionSet vset)
    {
        RouteGroupBuilder v1 = app.MapGroup("/api/v{version:apiVersion}/student")
            .WithApiVersionSet(vset)
            .RequireAuthorization("Student")
            .WithTags("Student Classes");

        v1.MapGet("/classes", GetClasses)
             .WithName("GetStudentClasses")
             .WithSummary("Retrieve classes for the authenticated student")
             .WithDescription("""
                Returns the list of classes that the authenticated student is enrolled in.  
                Supports cursor-based pagination.

                Query Parameters:
                - `cursor` (optional): The pagination cursor.
                - `limit` (optional): Maximum number of items to return.
                """)
             .Produces<CursorResult<ClassDto>>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status401Unauthorized)
             .Produces(StatusCodes.Status403Forbidden)
             .Produces(StatusCodes.Status500InternalServerError);


        v1.MapGet("/attendance", GetAttendance)
            .WithName("GetStudentAttendance")
            .WithSummary("Retrieve attendance records for the authenticated student")
            .WithDescription("Returns all attendance records for the authenticated student across all enrolled classes.")
            .Produces<List<ClassAttendanceDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAttendance(ISender sender)
    {
        Domain.Common.Results.Result<List<ClassAttendanceDto>> result = await sender.Send(new GetStudentAttendanceQuery());

        return result.Match(
        dto => Results.Ok(dto),
        errors => errors.ToProblem());
    }

    private static async Task<IResult> GetClasses([AsParameters] GetStudentClassesRequest request, ISender sender)
    {
        GetStudentClassesQuery query = new GetStudentClassesQuery
        {
            Cursor = request.Cursor,
            Limit = request.Limit,
        };

        Domain.Common.Results.Result<CursorResult<ClassDto>> result = await sender.Send(query);

        return result.Match(
           dto => Results.Ok(dto),
           errors => errors.ToProblem());
    }
}
