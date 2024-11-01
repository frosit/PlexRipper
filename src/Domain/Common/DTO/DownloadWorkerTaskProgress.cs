namespace PlexRipper.Domain;

public record DownloadWorkerTaskProgress : IDownloadTaskProgress
{
    public int Id { get; init; }

    public long DataTotal { get; init; }

    public decimal Percentage { get; init; }

    public long DataReceived { get; init; }

    public long DownloadSpeed { get; init; }

    public DownloadStatus Status { get; init; }
}
