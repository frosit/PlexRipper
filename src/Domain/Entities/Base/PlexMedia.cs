using System.ComponentModel.DataAnnotations.Schema;

namespace PlexRipper.Domain;

/// <summary>
///     Plex stores media in 1 generic type but PlexRipper stores it by type, this is the base entity for common
///     properties.
/// </summary>
public class PlexMedia : PlexMediaSlim
{
    #region Properties

    [Column(Order = 5)]
    public required string SearchTitle { get; init; }

    [Column(Order = 9)]
    public required string Studio { get; init; } = string.Empty;

    [Column(Order = 10)]
    public required string Summary { get; init; } = string.Empty;

    [Column(Order = 11)]
    public required string ContentRating { get; init; } = string.Empty;

    [Column(Order = 12)]
    public required double Rating { get; init; }

    /// <summary>
    /// Gets or sets when this media was released/aired to the public.
    /// </summary>
    [Column(Order = 16)]
    public required DateTime? OriginallyAvailableAt { get; init; }

    /// <summary>
    /// Gets or sets the full title path
    /// E.g. tvShow/Season/Episode
    /// TODO, might be better to remove this and make a getter for it.
    /// </summary>
    [Column(Order = 22)]
    public required string FullTitle { get; set; } = string.Empty;

    [Column(Order = 23)]
    public required string Guid { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the IMDB guid.
    /// Note: this is only the unique identifier part, and not including "imdb://".
    /// <example>imdb://imdb0397306</example>
    /// </summary>
    [Column(Order = 24)]
    public required string? Guid_IMDB { get; init; }

    /// <summary>
    /// Gets or sets the TMDB guid.
    /// Note: this is only the unique identifier part, and not including "tmdb://".
    /// <example>tmdb://1433</example>
    /// </summary>
    [Column(Order = 25)]
    public required string? Guid_TMDB { get; init; }

    /// <summary>
    /// Gets or sets the TVDB guid.
    /// Note: this is only the unique identifier part, and not including "tvdb://".
    /// <example>tvdb://73141</example>
    /// </summary>
    [Column(Order = 26)]
    public required string? Guid_TVDB { get; init; }

    #endregion

    #region Relationships

    public PlexLibrary? PlexLibrary { get; set; }

    public PlexServer? PlexServer { get; init; }

    #endregion
}
