using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Identity;
using SchoolMS.Application.Features.Identity.Commands.RefreshToken;
using SchoolMS.Application.Features.Identity.Commands.RegisterAdmin;
using SchoolMS.Application.Features.Identity.Commands.RegisterUser;
using SchoolMS.Application.Features.Identity.Queries.Login;
using SchoolMS.Contracts.Identity;

namespace SchoolMS.Api.Endpoints;

public static class IdentityEndpoints
{
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app, ApiVersionSet versions)
    {
        var v1 = app
            .MapGroup("/api/v{version:apiVersion}/auth")
            .WithApiVersionSet(versions)
            .MapToApiVersion(1.0)
            .WithTags("Authentication");

        v1.MapPost("/register", RegisterUser)
            .WithSummary("Register a new user")
            .WithDescription("Registers a new Student or Teacher account and returns JWT tokens.")
            .Accepts<RegisterUserRequest>("application/json")
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        v1.MapPost("/register-admin", RegisterAdmin)
            .RequireAuthorization("Admin")
            .WithSummary("Register a new administrator")
            .WithDescription("Creates an admin account. Requires Admin privileges.")
            .Accepts<RegisterAdminRequest>("application/json")
            .Produces(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        v1.MapPost("/refresh-token", RefreshToken)
            .WithSummary("Refresh access token")
            .WithDescription("Generates a new access token using a valid refresh token.")
            .Accepts<RefreshTokenRequest>("application/json")
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);

        v1.MapPost("/login", Login)
            .WithSummary("User login")
            .WithDescription("Authenticates a user and returns access/refresh tokens.")
            .Accepts<LoginRequest>("application/json")
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Login(LoginRequest request, ISender sender)
    {
        var loginQuery = new LoginQuery
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await sender.Send(loginQuery);

        return result.Match(
            value => Results.Ok(value),
            error => error.ToProblem());
    }

    private static async Task<IResult> RefreshToken(ISender sender, RefreshTokenRequest request, CancellationToken ct)
    {
        var command = new RefreshTokenCommand
        {
            ExpiredAccessToken = request.ExpiredAccessToken,
            RefreshToken = request.RefreshToken
        };

        var result = await sender.Send(command, ct);

        return result.Match(
            value => Results.Ok(value),
            error => error.ToProblem());
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
