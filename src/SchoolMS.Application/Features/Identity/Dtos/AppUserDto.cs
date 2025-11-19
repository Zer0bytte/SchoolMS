using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Identity.Dtos;

public record AppUserDto(Guid UserId, string Email, Role Role);


