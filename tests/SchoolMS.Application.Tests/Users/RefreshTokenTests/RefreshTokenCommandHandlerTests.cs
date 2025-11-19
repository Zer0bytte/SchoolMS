using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Identity;
using SchoolMS.Application.Features.Identity.Commands.RefreshToken;
using SchoolMS.Application.Features.Identity.Dtos;
using SchoolMS.Application.Tests.Shared;
using SchoolMS.Domain.RefreshTokens;
using SchoolMS.Domain.Users;
using SchoolMS.Domain.Users.Enums;
using System.Security.Claims;

namespace SchoolMS.Application.Tests.Users.RefreshTokenTests;

public class RefreshTokenCommandHandlerTests
{
    private TestAppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<TestAppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new TestAppDbContext(options);
    }

    [Fact]
    public async Task Handle_RefreshTokenExpired_ReturnsError()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var userId = Guid.NewGuid();
        var user = User.Create(userId, "Test User", "email@email.com", "hashedPassword", Role.Teacher).Value;
        context.Users.Add(user);

        await context.SaveChangesAsync(CancellationToken.None);

        var expiredRefresh = "refresh-old-token";

        var timeNow = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var expiresOn = timeNow.AddMinutes(5);

        var refreshToken = RefreshToken.Create(userId, expiredRefresh, userId, DateTimeOffset.UtcNow.AddMilliseconds(-1), timeNow);

        context.RefreshTokens.Add(refreshToken.Value);

        await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<RefreshTokenCommandHandler>>();
        var tokenProvider = new Mock<ITokenProvider>();

        tokenProvider.Setup(tp => tp.GetPrincipalFromExpiredToken(It.IsAny<string>()))
            .Returns(new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }
            )));

        var handler = new RefreshTokenCommandHandler(logger, context, tokenProvider.Object);

        var command = new RefreshTokenCommand
        {
            ExpiredAccessToken = "expired-access-token",
            RefreshToken = expiredRefresh
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.RefreshTokenExpired.Code, result.Errors.First().Code);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsNewTokens()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var userId = Guid.NewGuid();
        var validRefresh = "refresh-valid";

        // Add user
        var user = User.Create(userId, "Test User", "email@emai.com", "hashedPassword", Role.Teacher);

        context.Users.Add(user.Value);

        // Add refresh token
        context.RefreshTokens.Add(RefreshToken.Create(userId, validRefresh, userId, DateTime.UtcNow.AddMinutes(10), DateTimeOffset.UtcNow).Value);

        await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<RefreshTokenCommandHandler>>();
        var tokenProvider = new Mock<ITokenProvider>();


        tokenProvider.Setup(tp => tp.GetPrincipalFromExpiredToken(It.IsAny<string>()))
            .Returns(new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }
            )));


        var expectedTokenResponse = new TokenResponse { AccessToken = "newAccessToken", RefreshToken = "newRefreshToken" };

        tokenProvider.Setup(tp =>
                tp.GenerateJwtTokenAsync(It.IsAny<AppUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTokenResponse);

        var handler = new RefreshTokenCommandHandler(logger, context, tokenProvider.Object);

        var command = new RefreshTokenCommand
        {
            ExpiredAccessToken = "expired-access-token",
            RefreshToken = validRefresh
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("newAccessToken", result.Value.AccessToken);
        Assert.Equal("newRefreshToken", result.Value.RefreshToken);


        Assert.Equal(1, context.RefreshTokens.Count());
    }
}