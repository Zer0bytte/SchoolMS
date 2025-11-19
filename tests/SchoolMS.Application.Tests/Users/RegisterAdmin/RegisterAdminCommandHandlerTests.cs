using Microsoft.EntityFrameworkCore;
using Moq;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Identity.Commands.RegisterAdmin;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.Common.Results;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Tests.Users.RegisterAdmin;

public class RegisterAdminCommandHandlerTests
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

        var handler = new RegisterAdminCommandHandler(context, passwordHasher.Object);

        var command = new RegisterAdminCommand
        {
            Name = "New User",
            Email = "exists@school.test",
            Password = "password",
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);

        // Ensure no additional user was added
        Assert.Equal(1, context.Users.Count());
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesUserAndReturnsResultCreated()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = TestDbHelper.CreateContext();

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(ph => ph.HashPassword(It.IsAny<string>())).Returns((string p) => $"hashed-{p}");


        var handler = new RegisterAdminCommandHandler(context, passwordHasher.Object);

        var command = new RegisterAdminCommand
        {
            Name = "Alice Example",
            Email = "alice@example.test",
            Password = "SuperSecret",
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);


        // Assert user persisted
        var userInDb = context.Users.Single(u => u.Email == command.Email);
        Assert.Equal(command.Name, userInDb.Name);
        Assert.Equal($"hashed-{command.Password}", userInDb.Password);
        Assert.Equal(result.Value, Result.Created);
        Assert.Equal(Role.Admin, userInDb.Role);

    }
}