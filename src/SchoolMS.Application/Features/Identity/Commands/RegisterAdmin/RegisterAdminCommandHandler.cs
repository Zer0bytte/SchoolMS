using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Identity.Commands.RegisterAdmin;

public class RegisterAdminCommandHandler(IAppDbContext context, IPasswordHasher passwordHasher) : IRequestHandler<RegisterAdminCommand, Result<Created>>
{
    public async Task<Result<Created>> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
            return ApplicationErrors.EmailAlreadyExists;

        var user = User.Create(Guid.CreateVersion7(), request.Name, request.Email, passwordHasher.HashPassword(request.Password), Role.Admin);

        if (user.IsError) return user.Errors;

        context.Users.Add(user.Value);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Created;
    }
}
