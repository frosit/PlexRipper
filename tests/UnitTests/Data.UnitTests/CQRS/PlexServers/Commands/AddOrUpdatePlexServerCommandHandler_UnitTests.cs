using Microsoft.EntityFrameworkCore;
using PlexRipper.Application;
using PlexRipper.Data.PlexServers;

namespace Data.UnitTests.PlexServers.Commands;

public class AddOrUpdatePlexServerCommandHandler_UnitTests : BaseUnitTest
{
    public AddOrUpdatePlexServerCommandHandler_UnitTests(ITestOutputHelper output) : base(output, true) { }

    [Fact]
    public async Task ShouldAddAllServers_WhenNoneExistInTheDatabase()
    {
        // Arrange
        var expectedPlexServers = FakeData.GetPlexServer(config => config.Seed = 456324).Generate(5);

        // Act
        var request = new AddOrUpdatePlexServersCommand(expectedPlexServers);
        var handler = new AddOrUpdatePlexServersCommandHandler(DbContext);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        ResetDbContext();
        result.IsSuccess.ShouldBeTrue();
        var plexServersDbs = DbContext.PlexServers
            .Include(x => x.PlexServerConnections)
            .ToList();
        plexServersDbs.Count.ShouldBe(5);

        foreach (var expectedPlexServer in expectedPlexServers)
        {
            var plexServerDb = plexServersDbs.Find(x => x.MachineIdentifier == expectedPlexServer.MachineIdentifier);
            plexServerDb.ShouldNotBeNull();
            plexServerDb.ShouldBe(expectedPlexServer);
            plexServerDb.PlexServerConnections.Count.ShouldBe(expectedPlexServer.PlexServerConnections.Count);
            plexServerDb.PlexServerConnections.ShouldBe(expectedPlexServer.PlexServerConnections, true);
        }
    }

    [Fact]
    public async Task ShouldKeepTheSameServerConnectionIds_WhenOnlyTheConnectionPropertiesHaveChanged()
    {
        // Arrange
        var plexServers = FakeData.GetPlexServer(config => config.Seed = 23724).Generate(5);

        // Act
        // First Add 2
        var handler = new AddOrUpdatePlexServersCommandHandler(DbContext);
        var request = new AddOrUpdatePlexServersCommand(plexServers);
        var addResult = await handler.Handle(request, CancellationToken.None);

        // Update data setup
        var updatedServers = plexServers.Take(2).ToList();

        // Simulate the server being unchanged, and only the connections having changed except the connection
        // address which it is matched on during updating
        foreach (var updatedServer in updatedServers)
        {
            var connectionCount = updatedServer.PlexServerConnections.Count;
            var updatedConnections = FakeData.GetPlexServerConnections().Generate(connectionCount);
            for (var i = 0; i < connectionCount; i++)
                updatedConnections[i].Address = updatedServer.PlexServerConnections[i].Address;

            updatedServer.PlexServerConnections = updatedConnections;
        }

        // Now update
        ResetDbContext();
        handler = new AddOrUpdatePlexServersCommandHandler(DbContext);
        request = new AddOrUpdatePlexServersCommand(updatedServers);
        var updateResult = await handler.Handle(request, CancellationToken.None);

        // Assert
        addResult.IsSuccess.ShouldBeTrue();
        updateResult.IsSuccess.ShouldBeTrue();
        ResetDbContext();
        var plexServersDbs = DbContext.PlexServers
            .Include(x => x.PlexServerConnections)
            .ToList();
        plexServersDbs.Count.ShouldBe(5);

        foreach (var expectedServer in updatedServers)
        {
            var plexServerDb = plexServersDbs.Find(x => x.MachineIdentifier == expectedServer.MachineIdentifier);
            plexServerDb.ShouldNotBeNull();

            foreach (var plexServerConnectionDb in plexServerDb.PlexServerConnections)
            {
                var expectedConnection = expectedServer.PlexServerConnections.Find(x => x.Address == plexServerConnectionDb.Address);
                expectedConnection.ShouldNotBeNull();
                plexServerConnectionDb.Id.ShouldBe(expectedConnection.Id);
                plexServerConnectionDb.ShouldBe(expectedConnection);
            }
        }
    }

    [Fact]
    public async Task ShouldUpdateSomeAndSyncServersWithConnections_WhenSomeServerConnectionsHaveChangedAndSomeExistInTheDatabase()
    {
        // Arrange
        var plexServers = FakeData.GetPlexServer(config => config.Seed = 23724).Generate(5);
        var changedPlexServers = FakeData.GetPlexServer(config => config.Seed = 9236).Generate(3);

        var expectedPlexServers = new List<PlexServer>()
        {
            changedPlexServers[0],
            changedPlexServers[1],
            changedPlexServers[2],
            plexServers[3],
            plexServers[4],
        };

        // Create updated servers with the same machineId
        for (var i = 0; i < changedPlexServers.Count; i++)
            changedPlexServers[i].MachineIdentifier = plexServers[i].MachineIdentifier;

        // Act
        // First add the 5 servers
        var request = new AddOrUpdatePlexServersCommand(plexServers);
        var handler = new AddOrUpdatePlexServersCommandHandler(DbContext);
        var addResult = await handler.Handle(request, CancellationToken.None);

        // Now update
        ResetDbContext();
        request = new AddOrUpdatePlexServersCommand(changedPlexServers);
        handler = new AddOrUpdatePlexServersCommandHandler(DbContext);
        var updateResult = await handler.Handle(request, CancellationToken.None);

        // Assert
        ResetDbContext();
        addResult.IsSuccess.ShouldBeTrue();
        updateResult.IsSuccess.ShouldBeTrue();
        var plexServersDbs = DbContext.PlexServers
            .Include(x => x.PlexServerConnections)
            .ToList();
        plexServersDbs.Count.ShouldBe(5);

        foreach (var expectedPlexServer in expectedPlexServers)
        {
            var plexServerDb = plexServersDbs.Find(x => x.MachineIdentifier == expectedPlexServer.MachineIdentifier);
            plexServerDb.ShouldNotBeNull();
            plexServerDb.ShouldBe(expectedPlexServer);
            plexServerDb.PlexServerConnections.Count.ShouldBe(expectedPlexServer.PlexServerConnections.Count);
            plexServerDb.PlexServerConnections.ShouldBe(expectedPlexServer.PlexServerConnections, true);
        }
    }
}