namespace PlexRipper.Domain.DownloadV2;

public class DownloadTaskMovie : DownloadTaskParentBase
{
    #region Relationships

    public List<DownloadTaskMovieFile> Children { get; set; } = new();

    #endregion

    #region Helpers

    public override void SetNull()
    {
        base.SetNull();
        Children = null;
    }

    public override PlexMediaType MediaType => PlexMediaType.Movie;

    public override DownloadTaskType DownloadTaskType => DownloadTaskType.Movie;

    public override bool IsDownloadable => false;

    public override void Calculate()
    {
        if (!Children.Any())
            return;

        DataReceived = Children.Select(x => x.DataReceived).Sum();
        DataTotal = Children.Select(x => x.DataTotal).Sum();
        Percentage = DataFormat.GetPercentage(DataReceived, DataTotal);
    }

    #endregion
}