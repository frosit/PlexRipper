using Application.Contracts;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using PlexRipper.Domain;

namespace Data.Contracts;

public static partial class DbContextExtensions
{
    public static async Task<Result<int>> GetPlexMediaByMediaKeyAsync(
        this IPlexRipperDbContext dbContext,
        int plexMediaKey,
        int plexServerId,
        PlexMediaType mediaType,
        CancellationToken cancellationToken = default
    )
    {
        switch (mediaType)
        {
            case PlexMediaType.Movie:
            {
                var entity = await dbContext.PlexMovies.FirstOrDefaultAsync(
                    x => x.Key == plexMediaKey && x.PlexServerId == plexServerId,
                    cancellationToken
                );
                if (entity is not null)
                    return Result.Ok(entity.Id);

                break;
            }
            case PlexMediaType.TvShow:
            {
                var entity = await dbContext.PlexTvShows.FirstOrDefaultAsync(
                    x => x.Key == plexMediaKey && x.PlexServerId == plexServerId,
                    cancellationToken
                );
                if (entity is not null)
                    return Result.Ok(entity.Id);

                break;
            }
            case PlexMediaType.Season:
            {
                var entity = await dbContext.PlexTvShowSeason.FirstOrDefaultAsync(
                    x => x.Key == plexMediaKey && x.PlexServerId == plexServerId,
                    cancellationToken
                );
                if (entity is not null)
                    return Result.Ok(entity.Id);

                break;
            }
            case PlexMediaType.Episode:
            {
                var entity = await dbContext.PlexTvShowEpisodes.FirstOrDefaultAsync(
                    x => x.Key == plexMediaKey && x.PlexServerId == plexServerId,
                    cancellationToken
                );
                if (entity is not null)
                    return Result.Ok(entity.Id);

                break;
            }
            default:
                return Result.Fail($"Type {mediaType} is not supported for retrieving the plexMediaId by key");
        }

        return Result.Fail(
            $"Couldn't find a plexMediaId with key {plexMediaKey}, plexServerId {plexServerId} with type {mediaType}"
        );
    }

    public static async Task<Result<List<PlexMediaSlimDTO>>> GetMediaByType(
        this IPlexRipperDbContext dbContext,
        PlexMediaType mediaType,
        int skip = 0,
        int take = 0,
        int plexLibraryId = 0,
        bool filterOfflineMedia = false,
        bool filterOwnedMedia = false,
        CancellationToken ct = default
    )
    {
        List<PlexMediaSlimDTO> plexMediaSlimDtos;

        var serverList = await dbContext
            .PlexServers.Where(x => x.IsEnabled)
            .Select(server => new { Id = server.Id, PlexLibraryIds = server.PlexLibraries.Select(x => x.Id).ToList() })
            .ToListAsync(ct);

        var allowedPlexLibraryIds = serverList.SelectMany(x => x.PlexLibraryIds).ToList();
        if (filterOwnedMedia)
        {
            var ownedPlexLibraries = await dbContext
                .PlexAccountLibraries.Where(x => x.IsLibraryOwned)
                .Select(x => x.PlexLibraryId)
                .ToListAsync(ct);

            allowedPlexLibraryIds.RemoveAll(x => ownedPlexLibraries.Contains(x));
        }

        if (filterOfflineMedia)
        {
            foreach (var server in serverList)
            {
                var isServerOnline = await dbContext.IsServerOnline(server.Id, ct);
                if (!isServerOnline)
                {
                    allowedPlexLibraryIds.RemoveAll(x => server.PlexLibraryIds.Contains(x));
                }
            }
        }

        if (plexLibraryId == 0 && !allowedPlexLibraryIds.Any())
            return Result.Ok(new List<PlexMediaSlimDTO>());

        switch (mediaType)
        {
            case PlexMediaType.Movie:
            {
                plexMediaSlimDtos = await dbContext
                    .PlexMovies.AsNoTracking()
                    .ApplyWhere(plexLibraryId > 0, x => x.PlexLibraryId == plexLibraryId)
                    .ApplyWhere(plexLibraryId == 0, x => allowedPlexLibraryIds.Contains(x.PlexLibraryId))
                    .ApplyOrderBy(plexLibraryId > 0, x => x.SortIndex)
                    .ApplySkip(skip)
                    .ApplyTake(take)
                    .ProjectToMediaSlimDTO()
                    .ToListAsync(ct);

                break;
            }
            case PlexMediaType.TvShow:
            {
                plexMediaSlimDtos = await dbContext
                    .PlexTvShows.AsNoTracking()
                    .ApplyWhere(plexLibraryId > 0, x => x.PlexLibraryId == plexLibraryId)
                    .ApplyWhere(plexLibraryId == 0, x => allowedPlexLibraryIds.Contains(x.PlexLibraryId))
                    .ApplyOrderBy(plexLibraryId > 0, x => x.SortIndex)
                    .ApplySkip(skip)
                    .ApplyTake(take)
                    .ProjectToMediaSlimDTO()
                    .ToListAsync(ct);
                break;
            }
            default:
                return Result.Fail(
                    $"Type {mediaType} is not supported for retrieving the PlexMedia data by library id"
                );
        }

        if (plexLibraryId == 0)
        {
            plexMediaSlimDtos = plexMediaSlimDtos.OrderByNatural(x => x.SearchTitle).ToList();
        }

        // Add token to retrieve thumbnail in front-end
        Dictionary<int, string> tokensCache = new();

        for (var i = 0; i < plexMediaSlimDtos.Count; i++)
        {
            var slimDTO = plexMediaSlimDtos[i];

            slimDTO.SortIndex = i + 1;

            if (tokensCache.TryGetValue(slimDTO.PlexServerId, out var token))
            {
                slimDTO.PlexToken = token;
                continue;
            }

            var result = await dbContext.GetPlexServerTokenAsync(slimDTO.PlexServerId, ct);
            if (result.IsSuccess)
            {
                tokensCache.Add(slimDTO.PlexServerId, result.Value);
                slimDTO.PlexToken = result.Value;
            }
        }

        // If the plexLibraryId is set, we don't need to sort the list again
        return Result.Ok(plexMediaSlimDtos);
    }
}
