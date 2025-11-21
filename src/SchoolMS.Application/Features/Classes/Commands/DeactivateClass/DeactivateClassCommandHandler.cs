using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Classes.Commands.DeactivateClass;

public class DeactivateClassCommandHandler(IAppDbContext context, IUser user) : IRequestHandler<DeactivateClassCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeactivateClassCommand command, CancellationToken cancellationToken)
    {
        var cls = await context.Classes.FirstOrDefaultAsync(cls => cls.Id == command.Id && cls.IsActive);

        if (cls is null || cls.TeacherId != Guid.Parse(user.Id))
            return ClassErrors.NotFound;

        cls.Deactivate();

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success;

    }
}
