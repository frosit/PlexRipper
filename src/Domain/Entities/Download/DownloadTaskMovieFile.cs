namespace PlexRipper.Domain;

public class DownloadTaskMovieFile : DownloadTaskFileBase
{
    #region Relationships

    public required DownloadTaskMovie? Parent { get; init; }

    public required Guid ParentId { get; init; }

    #endregion

    #region Helpers

    public override int Count => 1;

    public override DownloadTaskKey ToParentKey() =>
        new()
        {
            Type = DownloadTaskType.Movie,
            Id = ParentId,
            PlexServerId = PlexServerId,
            PlexLibraryId = PlexLibraryId,
        };

    public override PlexMediaType MediaType => PlexMediaType.Movie;

    public override DownloadTaskType DownloadTaskType => DownloadTaskType.MovieData;

    public override bool IsDownloadable => true;

    #endregion
}
