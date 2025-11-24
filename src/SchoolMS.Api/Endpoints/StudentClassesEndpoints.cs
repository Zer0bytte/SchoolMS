using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Classes.Queries.Student.GetStudentAttendance;
using SchoolMS.Application.Features.Classes.Queries.Student.GetStudentClasses;
using SchoolMS.Contracts.Classes;

namespace SchoolMS.Api.Endpoints;

public static class StudentClassesEndpoints
{
    public static void MapStudentClassesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/student").RequireAuthorization("Student");

        group.MapGet("/classes", GetClasses);
        group.MapGet("/attendance", GetAttendance);
    }

    private static async Task<IResult> GetAttendance(ISender sender)
    {
        var result = await sender.Send(new GetStudentAttendanceQuery());

        return result.Match(
        dto => Results.Ok(dto),
        errors => errors.ToProblem());
    }

    private static async Task<IResult> GetClasses([AsParameters] GetStudentClassesRequest request, ISender sender)
    {
        var query = new GetStudentClassesQuery
        {
            Cursor = request.Cursor,
            Limit = request.Limit,
        };

        var result = await sender.Send(query);

        return result.Match(
           dto => Results.Ok(dto),
           errors => errors.ToProblem());
    }
}
