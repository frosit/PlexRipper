using Data.Contracts;
using FluentValidation;
using Logging.Interface;
using Microsoft.EntityFrameworkCore;

namespace PlexRipper.Application;

public record AddOrUpdatePlexServersCommand(List<PlexServer> PlexServers) : IRequest<Result>;

public class AddOrUpdatePlexServersCommandValidator : AbstractValidator<AddOrUpdatePlexServersCommand>
{
    public AddOrUpdatePlexServersCommandValidator()
    {
        RuleFor(x => x.PlexServers).NotEmpty().WithMessage("PlexServers list cannot be empty.");

        RuleForEach(x => x.PlexServers)
            .ChildRules(server =>
            {
                server
                    .RuleForEach(s => s.PlexServerConnections)
                    .ChildRules(connection =>
                    {
                        connection.RuleFor(c => c.Protocol).NotEmpty().WithMessage("Protocol is required.");

                        connection.RuleFor(c => c.Address).NotEmpty().WithMessage("Address is required.");

                        connection.RuleFor(c => c.Port).NotEmpty().WithMessage("Port is required.");
                        connection
                            .RuleFor(c => c.PlexServerStatus)
                            .Empty()
                            .WithMessage("PlexServerStatus should be empty.");
                    });
            });
    }
}

public class AddOrUpdatePlexServersCommandHandler : IRequestHandler<AddOrUpdatePlexServersCommand, Result>
{
    private readonly ILog _log;
    private readonly IPlexRipperDbContext _dbContext;

    public AddOrUpdatePlexServersCommandHandler(ILog log, IPlexRipperDbContext dbContext)
    {
        _log = log;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(AddOrUpdatePlexServersCommand command, CancellationToken cancellationToken)
    {
        var plexServers = command.PlexServers;

        // Add or update the PlexServers in the database
        _log.Information("Adding or updating {PlexServersCount} PlexServers now", plexServers.Count);

        var machineIds = plexServers.Select(x => x.MachineIdentifier).ToList();
        var plexServerDbList = await _dbContext
            .PlexServers.Include(x => x.PlexServerConnections)
            .Where(x => machineIds.Contains(x.MachineIdentifier))
            .AsTracking()
            .ToListAsync(cancellationToken);

        foreach (var plexServer in plexServers)
        {
            var existingServer = plexServerDbList.FirstOrDefault(x =>
                x.MachineIdentifier == plexServer.MachineIdentifier
            );
            if (existingServer != null)
            {
                // PlexServer already exists
                _log.Debug("Updating PlexServer with id: {PlexServerDbId} in the database", existingServer.Id);
                plexServer.Id = existingServer.Id;

                _dbContext.Entry(existingServer).CurrentValues.SetValues(plexServer);

                SyncPlexServerConnections(plexServer, existingServer);
            }
            else
            {
                // Create plexServer
                _log.Debug("Adding PlexServer with name: {PlexServerName} to the database", plexServer.Name);
                foreach (var plexServerConnection in plexServer.PlexServerConnections)
                    plexServerConnection.PlexServerId = plexServer.Id;

                _dbContext.PlexServers.Add(plexServer);
                _dbContext.PlexServerConnections.AddRange(plexServer.PlexServerConnections);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    private void SyncPlexServerConnections(PlexServer plexServer, PlexServer existingServer)
    {
        // Create or Update PlexServerConnections
        foreach (var newConnection in plexServer.PlexServerConnections)
        {
            newConnection.PlexServerId = plexServer.Id;

            var existingConnection = existingServer.PlexServerConnections.FirstOrDefault(x => x.Equals(newConnection));

            if (existingConnection is null)
            {
                _log.Here()
                    .Debug(
                        "Creating connection {PlexServerConnection} from {PlexServerName} in the database",
                        newConnection.ToString(),
                        existingServer.Name
                    );
                _dbContext.PlexServerConnections.Add(newConnection);
            }
            else
            {
                _log.Here()
                    .Debug(
                        "Updating connection {PlexServerConnection} from {PlexServerName} in the database",
                        newConnection.ToString(),
                        existingServer.Name
                    );
                newConnection.Id = existingConnection.Id;
                _dbContext.Entry(existingConnection).CurrentValues.SetValues(newConnection);
            }
        }

        foreach (var existingConnection in existingServer.PlexServerConnections.ToList())
        {
            if (
                !existingConnection.IsCustom && !plexServer.PlexServerConnections.Any(x => x.Equals(existingConnection))
            )
            {
                _log.Here()
                    .Debug(
                        "Removing connection {PlexServerConnection} from {PlexServerName} in the database",
                        existingConnection.ToString(),
                        existingServer.Name
                    );
                _dbContext.Entry(existingConnection).State = EntityState.Deleted;
            }
        }
    }
}
