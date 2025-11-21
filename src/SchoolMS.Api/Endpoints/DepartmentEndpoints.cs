
using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Departments.Commands.CreateDepartment;
using SchoolMS.Application.Features.Departments.Commands.DeleteDepartment;
using SchoolMS.Application.Features.Departments.Commands.UpdateDepartment;
using SchoolMS.Application.Features.Departments.Dtos;
using SchoolMS.Application.Features.Departments.Queries.GetDepartment;
using SchoolMS.Application.Features.Departments.Queries.GetDepartments;
using SchoolMS.Contracts.Departments;

namespace SchoolMS.Api.Endpoints;

public static class DepartmentEndpoints
{
    public static void MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/departments").RequireAuthorization("Admin").WithTags("Departments");

        group.MapGet("", GetDepartments)
              .WithSummary("Get all departments")
              .WithDescription("Returns a paginated/filterable list of departments. Only accessible by Admin users.")
              .Produces<DepartmentsResult>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetDepartment)
            .WithSummary("Get department by ID")
            .WithDescription("Returns a specific department using its unique identifier. Only accessible by Admin users.")
            .Produces<DepartmentsResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("", CreateDepartment)
            .WithSummary("Create a new department")
            .WithDescription("Creates a new department record. Only accessible by Admin users.")
            .Accepts<CreateDepartmentRequest>("application/json")
            .Produces<DepartmentDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPatch("/{id:guid}", UpdateDepartment)
            .WithSummary("Update a department")
            .WithDescription("Updates an existing department using its ID. Only accessible by Admin users.")
            .Accepts<UpdateDepartmentRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", DeleteDepartment)
            .WithSummary("Delete a department")
            .WithDescription("Deletes an existing department using its ID. Only accessible by Admin users.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);


    }

    private static async Task<IResult> GetDepartment(Guid id, ISender sender)
    {
        var query = new GetDepartmentByIdQuery
        {
            Id = id
        };

        var result = await sender.Send(query);

        return result.Match(result => Results.Ok(result), error => error.ToProblem());
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
