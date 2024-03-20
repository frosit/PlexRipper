﻿using Application.Contracts;

namespace PlexRipper.Application.UnitTests;

public class CreatePlexAccountCommand_UnitTests : BaseUnitTest<CreatePlexAccountHandler>
{
    public CreatePlexAccountCommand_UnitTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task CreatePlexAccountAsync_ShouldSuccessResult_WhenAccountIsValid()
    {
        // Arrange
        await SetupDatabase();
        var newAccount = new PlexAccount("TestUsername", "Password123");

        mock.SetupMediator(It.IsAny<InspectAllPlexServersByAccountIdCommand>).ReturnsAsync(Result.Ok());

        // Act
        var handler = new CreatePlexAccountHandler(_log, GetDbContext(), mock.Create<IMediator>());
        var result = await handler.Handle(new CreatePlexAccountCommand(newAccount), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task CreatePlexAccountAsync_ShouldFailedResult_WhenAccountAlreadyExists()
    {
        // Arrange
        await SetupDatabase();
        var newAccount = new PlexAccount("TestUsername", "Password123");

        // Act
        var handler = new CreatePlexAccountHandler(_log, GetDbContext(), mock.Create<IMediator>());
        var result = await handler.Handle(new CreatePlexAccountCommand(newAccount), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task CreatePlexAccountAsync_ShouldFailedResult_WhenAccountUsernameExistenceCheckFailed()
    {
        // Arrange
        await SetupDatabase();
        var newAccount = new PlexAccount("TestUsername", "Password123");

        // Act
        var handler = new CreatePlexAccountHandler(_log, GetDbContext(), mock.Create<IMediator>());
        var result = await handler.Handle(new CreatePlexAccountCommand(newAccount), CancellationToken.None);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldBe("Error #1");
    }

    [Fact]
    public async Task CreatePlexAccountAsync_ShouldFailedResult_WhenAccountCreationFailed()
    {
        // Arrange
        await SetupDatabase();
        var newAccount = new PlexAccount("TestUsername", "Password123");

        // Act
        var handler = new CreatePlexAccountHandler(_log, GetDbContext(), mock.Create<IMediator>());
        var result = await handler.Handle(new CreatePlexAccountCommand(newAccount), CancellationToken.None);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldBe("Error #1");
    }
}