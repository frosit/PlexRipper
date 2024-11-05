using Application.Contracts;

namespace PlexRipper.Application;

public record ServerOnlineStatusChangedNotification : INotification
{
    public ServerOnlineStatusChangedNotification(int plexServerId, bool isOnline)
    {
        PlexServerId = plexServerId;
        IsOnline = isOnline;
    }

    public int PlexServerId { get; }

    public bool IsOnline { get; }
}

public class ServerOnlineStatusChangedHandler : INotificationHandler<ServerOnlineStatusChangedNotification>
{
    private readonly IDownloadQueue _downloadQueue;

    public ServerOnlineStatusChangedHandler(IDownloadQueue downloadQueue)
    {
        _downloadQueue = downloadQueue;
    }

    public async Task Handle(ServerOnlineStatusChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.IsOnline)
        {
            await _downloadQueue.CheckDownloadQueue([notification.PlexServerId]);
        }
    }
}
