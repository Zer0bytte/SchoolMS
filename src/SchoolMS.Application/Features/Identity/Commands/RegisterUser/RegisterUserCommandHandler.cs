using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.Users;

namespace SchoolMS.Application.Features.Identity.Commands.RegisterUser;

public class RegisterUserCommandHandler(IAppDbContext context, IPasswordHasher passwordHasher, ITokenProvider tokenProvider)
    : IRequestHandler<RegisterUserCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            return ApplicationErrors.EmailAlreadyExists;

        var user = User.Create(Guid.CreateVersion7(), request.Name, request.Email, passwordHasher.HashPassword(request.Password), request.Role);

        if (user.IsError) return user.Errors;

        context.Users.Add(user.Value);

        await context.SaveChangesAsync(cancellationToken);

        var token = await tokenProvider.GenerateJwtTokenAsync(user.Value, cancellationToken);

        return token;
    }
}
