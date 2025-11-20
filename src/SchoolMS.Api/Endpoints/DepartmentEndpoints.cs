
using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Departments.Commands.CreateDepartment;
using SchoolMS.Application.Features.Departments.Commands.RemoveDepartment;
using SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;
using SchoolMS.Application.Features.Departments.Queries.GetDepartments;
using SchoolMS.Contracts.Departments;

namespace SchoolMS.Api.Endpoints;

public static class DepartmentEndpoints
{
    public static void MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/departments").RequireAuthorization("Admin");

        group.MapGet("", GetDepartments);
        group.MapGet("/{id:guid}", GetDepartment);

        group.MapPost("", CreateDepartment);
        group.MapPatch("/{id:guid}", UpdateDepartment);
        group.MapDelete("/{id:guid}", DeleteDepartment);


    }

    private static async Task GetDepartment(HttpContext context)
    {

    }

    private static async Task<IResult> DeleteDepartment(Guid id, ISender sender)
    {
        var command = new DeleteDepartmentCommand
        {
            DepartmentId = id
        };

        var result = await sender.Send(command);

        return result.Match(result => Results.NoContent(), error => error.ToProblem());
    }
    private static async Task<IResult> UpdateDepartment(Guid id, UpdateDepartmentRequest request, ISender sender)
    {
        var command = new UpdateDepartmentCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            HeadOfDepartmentId = request.HeadOfDepartmentId
        };

        var result = await sender.Send(command);

        return result.Match(result => Results.NoContent(), error => error.ToProblem());
    }

    private static async Task<IResult> CreateDepartment(CreateDepartmentRequest request, ISender sender)
    {
        var command = new CreateDepartmentCommand
        {
            Name = request.Name,
            Description = request.Description,
            HeadOfDepartmentId = request.HeadOfDepartmentId
        };

        var result = await sender.Send(command);

        return result.Match(result => Results.Created("/departments/" + result.Id, result), error => error.ToProblem());
    }

    private static async Task<IResult> GetDepartments([AsParameters] GetDepartmentsRequest request, ISender sender)
    {
        var query = new GetDepartmentsQuery
        {
            Cursor = request.Cursor,
            DepartmentName = request.DepartmentName,
            Limit = request.Limit
        };

        var result = await sender.Send(query);

        return result.Match(result => Results.Ok(result), error => error.ToProblem());
    }
}
