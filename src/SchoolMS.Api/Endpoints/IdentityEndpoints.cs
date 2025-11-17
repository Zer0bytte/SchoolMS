
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Identity.Commands.RegisterAdmin;
using SchoolMS.Application.Features.Identity.Commands.RegisterUser;
using SchoolMS.Contracts.Identity;

namespace SchoolMS.Api.Endpoints;

public static class IdentityEndpoints
{
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity").WithTags("Identity");

        group.MapPost("/register", RegisterUser);
        group.MapPost("/register-admin", RegisterAdmin).RequireAuthorization("Admin");
    }

    private static async Task<IResult> RegisterAdmin(RegisterAdminRequest request, ISender sender)
    {
        var command = new RegisterAdminCommand
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
        };

        var result = await sender.Send(command);


        return result.Match(
           tokenResponse => Results.Created(),
           errors => errors.ToProblem()
       );
    }

    private static async Task<IResult> RegisterUser(RegisterUserRequest request, ISender sender)
    {
        var command = new RegisterUserCommand
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password,
            Role = request.Role
        };

        var result = await sender.Send(command);

        return result.Match(
            tokenResponse => Results.Ok(tokenResponse),
            errors => errors.ToProblem()
        );
    }
}
