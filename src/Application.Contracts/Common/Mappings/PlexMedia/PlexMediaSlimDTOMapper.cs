using PlexRipper.Domain;

namespace Application.Contracts;

public static class PlexMediaSlimDTOMapper
{
    public static PlexMediaSlimDTO ToSlimDTO(this PlexMediaSlim source) =>
        new()
        {
            Id = source.Id,
            Title = source.Title,
            SortIndex = source.SortIndex,
            Year = source.Year,
            Duration = source.Duration,
            MediaSize = source.MediaSize,
            ChildCount = source.ChildCount,
            GrandChildCount = 0,
            AddedAt = source.AddedAt,
            UpdatedAt = source.UpdatedAt,
            PlexLibraryId = source.PlexLibraryId,
            PlexServerId = source.PlexServerId,
            Type = source.Type,
            HasThumb = source.HasThumb,
            Qualities = source.Qualities.ToDTO(),
            SearchTitle = string.Empty, // TODO Missing in PlexMediaSlim
            Key = source.Key,
            MetaDataKey = source.MetaDataKey,
            PlexToken = string.Empty,
        };

    #region PlexMovie

    public static PlexMediaSlimDTO ToSlimDTO(this PlexMovie source) =>
        new()
        {
            Id = source.Id,
            Title = source.Title,
            SortIndex = source.SortIndex,
            Year = source.Year,
            SearchTitle = source.SearchTitle,
            Duration = source.Duration,
            MediaSize = source.MediaSize,
            ChildCount = source.ChildCount,
            AddedAt = source.AddedAt,
            UpdatedAt = source.UpdatedAt,
            PlexLibraryId = source.PlexLibraryId,
            PlexServerId = source.PlexServerId,
            Type = source.Type,
            HasThumb = source.HasThumb,
            GrandChildCount = 0,
            Qualities = source.Qualities.ToDTO(),
            Key = source.Key,
            MetaDataKey = source.MetaDataKey,
            PlexToken = string.Empty,
        };

    public static IQueryable<PlexMediaSlimDTO> ProjectToMediaSlimDTO(this IQueryable<PlexMovie> source) =>
        source.Select(x => ToSlimDTO(x));

    #endregion

    #region PlexTvShow

    public static IQueryable<PlexMediaSlimDTO> ProjectToMediaSlimDTO(this IQueryable<PlexTvShow> source) =>
        source.Select(x => ToSlimDTOMapper(x));

    private static PlexMediaSlimDTO ToSlimDTOMapper(this PlexTvShow source) =>
        new()
        {
            Id = source.Id,
            Title = source.Title,
            SearchTitle = source.SearchTitle,
            SortIndex = source.SortIndex,
            Year = source.Year,
            Duration = source.Duration,
            MediaSize = source.MediaSize,
            ChildCount = source.ChildCount,
            GrandChildCount = source.GrandChildCount,
            AddedAt = source.AddedAt,
            UpdatedAt = source.UpdatedAt,
            PlexLibraryId = source.PlexLibraryId,
            PlexServerId = source.PlexServerId,
            Type = source.Type,
            Key = source.Key,
            MetaDataKey = source.MetaDataKey,
            HasThumb = source.HasThumb,
            Qualities = source.Qualities.ToDTO(),
            PlexToken = string.Empty,
        };

    #endregion

    #region PlexSeason

    public static IQueryable<PlexMediaSlimDTO> ProjectToMediaSlimDTO(this IQueryable<PlexTvShowSeason> source) =>
        source.Select(x => ToSlimDTO(x));

    private static PlexMediaSlimDTO ToSlimDTOMapper(this PlexTvShowSeason source) =>
        new()
        {
            Id = source.Id,
            Title = source.Title,
            SearchTitle = source.SearchTitle,
            SortIndex = source.SortIndex,
            Year = source.Year,
            Duration = source.Duration,
            MediaSize = source.MediaSize,
            ChildCount = source.ChildCount,
            GrandChildCount = 0,
            AddedAt = source.AddedAt,
            UpdatedAt = source.UpdatedAt,
            PlexLibraryId = source.PlexLibraryId,
            PlexServerId = source.PlexServerId,
            Type = source.Type,
            Key = source.Key,
            MetaDataKey = source.MetaDataKey,
            HasThumb = source.HasThumb,
            Qualities = source.Qualities.ToDTO(),
            PlexToken = string.Empty,
        };

    #endregion

    #region PlexEpisode

    public static PlexMediaSlimDTO ToSlimDTO(this PlexTvShowEpisode source) =>
        new()
        {
            Id = source.Id,
            Title = source.Title,
            SearchTitle = source.SearchTitle,
            SortIndex = source.SortIndex,
            Year = source.Year,
            Duration = source.Duration,
            MediaSize = source.MediaSize,
            ChildCount = source.ChildCount,
            GrandChildCount = 0,
            AddedAt = source.AddedAt,
            UpdatedAt = source.UpdatedAt,
            PlexLibraryId = source.PlexLibraryId,
            PlexServerId = source.PlexServerId,
            Type = source.Type,
            HasThumb = source.HasThumb,
            Key = source.Key,
            MetaDataKey = source.MetaDataKey,
            PlexToken = string.Empty,
            Qualities = source.Qualities.ToDTO(),
        };

    public static IQueryable<PlexMediaSlimDTO> ProjectToMediaSlimDTO(this IQueryable<PlexTvShowEpisode> source) =>
        source.Select(x => ToSlimDTO(x));

    #endregion
}
