namespace PlexRipper.Domain;

public class PlexTvShow : PlexMedia
{
    public override PlexMediaType Type => PlexMediaType.TvShow;

    public required int GrandChildCount { get; set; }

    #region Relationships

    public List<PlexTvShowSeason> Seasons { get; set; } = [];

    #endregion
}
