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
        int skip,
        int take,
        int plexLibraryId = 0,
        CancellationToken ct = default
    )
    {
        var entities = new List<PlexMediaSlimDTO>();

        switch (mediaType)
        {
            case PlexMediaType.Movie:
            {
                entities = await dbContext
                    .PlexMovies.AsNoTracking()
                    .Where(x => x.PlexLibraryId <= 0 || x.PlexLibraryId == plexLibraryId)
                    .OrderBy(x => x.SortIndex)
                    .Skip(skip)
                    .Take(take)
                    .ProjectToMediaSlimDTO()
                    .ToListAsync(ct);

                break;
            }
            case PlexMediaType.TvShow:
            {
                entities = await dbContext
                    .PlexTvShows.AsNoTracking()
                    .Where(x => x.PlexLibraryId <= 0 || x.PlexLibraryId == plexLibraryId)
                    .OrderBy(x => x.SortIndex)
                    .Skip(skip)
                    .Take(take)
                    .ProjectToMediaSlimDTO()
                    .ToListAsync(ct);
                break;
            }
            default:
                return Result.Fail(
                    $"Type {mediaType} is not supported for retrieving the PlexMedia data by library id"
                );
        }

        // Add token to retrieve thumbnail in front-end
        Dictionary<int, string> tokensCache = new();

        foreach (var slimDTO in entities)
        {
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

        return Result.Ok(entities);
    }
}
