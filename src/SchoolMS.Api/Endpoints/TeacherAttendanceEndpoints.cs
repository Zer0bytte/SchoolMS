using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Classes.Commands.MarkAttendance;
using SchoolMS.Application.Features.Classes.Queries.GetClassAttendance;
using SchoolMS.Contracts.Classes;

namespace SchoolMS.Api.Endpoints;

public static class TeacherAttendanceEndpoints
{
    public static void MapTeacherAttendanceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/teacher/attendance");

        group.MapPost("", MarkAttendance);
        group.MapPost("/{classId:guid}", GetAttendanceHistory);
    }

    private static async Task<IResult> GetAttendanceHistory(Guid classId, ISender sender)
    {
        var query = new GetClassAttendanceQuery
        {
            ClassId = classId,
        };

        var result = await sender.Send(query);

        return result.Match
               (result => Results.Ok(result),
               error => error.ToProblem());
    }

    private static async Task<IResult> MarkAttendance(MarkAttendanceRequest request, ISender sender)
    {
        var command = new MarkAttendanceCommand
        {
            ClassId = request.ClassId,
            Students = request.Students,
        };

        var result = await sender.Send(command);

        return result.Match
                 (result => Results.NoContent(),
                 error => error.ToProblem());
    }
}
