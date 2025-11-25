using Asp.Versioning.Builder;
using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Classes.Commands.MarkAttendance;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Application.Features.Classes.Queries.Teacher.GetClassAttendance;
using SchoolMS.Contracts.Classes;

namespace SchoolMS.Api.Endpoints;

public static class TeacherAttendanceEndpoints
{
    public static void MapTeacherAttendanceEndpoints(this IEndpointRouteBuilder app, ApiVersionSet vset)
    {
        RouteGroupBuilder v1 = app.MapGroup("/api/v{version:apiVersion}/teacher/attendance")
            .WithApiVersionSet(vset)
            .RequireAuthorization("Teacher")
            .WithTags("Teacher Attendance");


        v1.MapPost("", MarkAttendance)
            .WithName("MarkAttendance")
            .WithSummary("Mark attendance for students in a class")
            .WithDescription("""
                Allows the authenticated teacher to mark attendance for students in a specific class.  
                The request should include the Class ID and a list of students with their attendance status.
                """)
            .Accepts<MarkAttendanceRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);

        v1.MapGet("/{classId:guid}", GetAttendanceHistory)
            .WithName("GetClassAttendance")
            .WithSummary("Retrieve attendance history for a class")
            .WithDescription("Returns the attendance records for all students in the specified class.")
            .Produces<List<StudentAttendanceEntry>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetAttendanceHistory(Guid classId, ISender sender)
    {
        GetClassAttendanceQuery query = new GetClassAttendanceQuery
        {
            ClassId = classId,
        };

        Domain.Common.Results.Result<List<StudentAttendanceEntry>> result = await sender.Send(query);

        return result.Match
               (result => Results.Ok(result),
               error => error.ToProblem());
    }

    private static async Task<IResult> MarkAttendance(MarkAttendanceRequest request, ISender sender)
    {
        MarkAttendanceCommand command = new MarkAttendanceCommand
        {
            ClassId = request.ClassId,
            Students = request.Students,
        };

        Domain.Common.Results.Result<Domain.Common.Results.Success> result = await sender.Send(command);

        return result.Match
                 (result => Results.NoContent(),
                 error => error.ToProblem());
    }
}
