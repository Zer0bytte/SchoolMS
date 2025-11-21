using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Identity;
using SchoolMS.Application.Features.Identity.Commands.RegisterUser;
using SchoolMS.Application.Features.Identity.Dtos;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Tests.UserTests.RegisterUser;

public class RegisterUserCommandHandlerTests
{


    [Fact]
    public async Task Handle_EmailAlreadyExists_ReturnsError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();

        var existingUserResult = User.Create(Guid.NewGuid(), "Existing User", "exists@school.test", "plain", Role.Student);
        Assert.False(existingUserResult.IsError);
        context.Users.Add(existingUserResult.Value);
        await context.SaveChangesAsync(CancellationToken.None);

        var passwordHasher = new Mock<IPasswordHasher>();
        var tokenProvider = new Mock<ITokenProvider>();

        var handler = new RegisterUserCommandHandler(context, passwordHasher.Object, tokenProvider.Object);

        var command = new RegisterUserCommand
        {
            Name = "New User",
            Email = "exists@school.test",
            Password = "password",
            Role = Role.Student
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        // Ensure no token was generated
        tokenProvider.Verify(tp => tp.GenerateJwtTokenAsync(It.IsAny<AppUserDto>(), It.IsAny<CancellationToken>()), Times.Never);
        // Ensure no additional user was added
        Assert.Equal(1, context.Users.Count());
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesUserAndReturnsToken()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(ph => ph.HashPassword(It.IsAny<string>())).Returns((string p) => $"hashed-{p}");

        var expectedToken = new TokenResponse
        {
            AccessToken = "access-token-value",
            ExpiresOnUtc = DateTime.UtcNow.AddHours(1)
        };

        var tokenProvider = new Mock<ITokenProvider>();
        tokenProvider
            .Setup(tp => tp.GenerateJwtTokenAsync(It.IsAny<AppUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Result<TokenResponse>)expectedToken);

        var handler = new RegisterUserCommandHandler(context, passwordHasher.Object, tokenProvider.Object);

        var command = new RegisterUserCommand
        {
            Name = "Alice Example",
            Email = "alice@example.test",
            Password = "SuperSecret",
            Role = Role.Teacher
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert token result
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedToken.AccessToken, result.Value.AccessToken);

        // Assert user persisted
        var userInDb = context.Users.Single(u => u.Email == command.Email);
        Assert.Equal(command.Name, userInDb.Name);
        Assert.Equal($"hashed-{command.Password}", userInDb.Password);
        Assert.Equal(command.Role, userInDb.Role);

        tokenProvider.Verify(tp => tp.GenerateJwtTokenAsync(It.Is<AppUserDto>(u => u.Email == command.Email), It.IsAny<CancellationToken>()), Times.Once);
    }
}