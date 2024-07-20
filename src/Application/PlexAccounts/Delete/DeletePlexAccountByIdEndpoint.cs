using Application.Contracts;
using Data.Contracts;
using FastEndpoints;
using FluentValidation;
using Logging.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace PlexRipper.Application;

public record DeletePlexAccountByIdRequest(int PlexAccountId);

public class DeletePlexAccountByIdRequestValidator : Validator<DeletePlexAccountByIdRequest>
{
    public DeletePlexAccountByIdRequestValidator()
    {
        RuleFor(x => x.PlexAccountId).GreaterThan(0);
    }
}

public class DeletePlexAccountByIdEndpoint : BaseEndpoint<DeletePlexAccountByIdRequest>
{
    private readonly ILog _log;
    private readonly IPlexRipperDbContext _dbContext;

    public override string EndpointPath => ApiRoutes.PlexAccountController + "/{PlexAccountId}";

    public DeletePlexAccountByIdEndpoint(ILog log, IPlexRipperDbContext dbContext)
    {
        _log = log;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Delete(EndpointPath);
        AllowAnonymous();
        Description(x =>
            x.Produces(StatusCodes.Status200OK, typeof(ResultDTO))
                .Produces(StatusCodes.Status400BadRequest, typeof(ResultDTO))
                .Produces(StatusCodes.Status404NotFound, typeof(ResultDTO))
                .Produces(StatusCodes.Status500InternalServerError, typeof(ResultDTO))
        );
    }

    public override async Task HandleAsync(DeletePlexAccountByIdRequest req, CancellationToken ct)
    {
        var deletedPlexAccountsCount = await _dbContext
            .PlexAccounts.Where(x => x.Id == req.PlexAccountId)
            .ExecuteDeleteAsync(ct);

        if (deletedPlexAccountsCount == 0)
        {
            await SendFluentResult(
                Result.Fail($"Could not find {nameof(PlexAccount)} with id {req.PlexAccountId} to delete.").LogError(),
                ct
            );
            return;
        }

        // Clean up orphaned PlexServers and PlexLibraries
        var accessibleServerIds = await _dbContext.PlexAccountServers.Select(y => y.PlexServerId).ToListAsync(ct);
        var accessibleLibraryIds = await _dbContext.PlexAccountLibraries.Select(y => y.PlexLibraryId).ToListAsync(ct);

        var deletedServersCount = await _dbContext
            .PlexServers.Where(x => !accessibleServerIds.Contains(x.Id))
            .ExecuteDeleteAsync(ct);

        var deletedLibrariesCount = await _dbContext
            .PlexLibraries.Where(x => !accessibleLibraryIds.Contains(x.Id))
            .ExecuteDeleteAsync(ct);

        _log.Debug(
            "Deleted {PlexAccount} with Id: {CommandId} from the database, and cleaned up {DeletedServersCount} PlexServers and {DeletedLibrariesCount} PlexLibraries",
            nameof(PlexAccount),
            req.PlexAccountId,
            deletedServersCount,
            deletedLibrariesCount
        );

        await SendFluentResult(Result.Ok(), ct);
    }
}
