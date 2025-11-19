using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Identity.Dtos;
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
        var userDto = new AppUserDto
       (
            user.Value.Id,
            user.Value.Email,
            user.Value.Role
            );
        var token = await tokenProvider.GenerateJwtTokenAsync(userDto, cancellationToken);

        return token;
    }
}
