namespace SchoolMS.Application.Features.Classes.Commands.DeactivateClass;

public class DeactivateClassCommand : IRequest<Result<Success>>
{
    public Guid Id { get; set; }
}
