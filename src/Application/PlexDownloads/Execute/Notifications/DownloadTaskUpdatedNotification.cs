using Application.Contracts;
using Data.Contracts;
using FileSystem.Contracts;

namespace PlexRipper.Application;

public record DownloadTaskUpdatedNotification(DownloadTaskKey Key) : IRequest;

public class DownloadTaskUpdatedHandler : IRequestHandler<DownloadTaskUpdatedNotification>
{
    private readonly IPlexRipperDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly ISignalRService _signalRService;
    private readonly IFileMergeScheduler _fileMergeScheduler;

    public DownloadTaskUpdatedHandler(
        IPlexRipperDbContext dbContext,
        IMediator mediator,
        ISignalRService signalRService,
        IFileMergeScheduler fileMergeScheduler
    )
    {
        _dbContext = dbContext;
        _mediator = mediator;
        _signalRService = signalRService;
        _fileMergeScheduler = fileMergeScheduler;
    }

    public async Task Handle(DownloadTaskUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var plexServerId = notification.Key.PlexServerId;

        // Ensure the up-to-date download status is written to the database as the DownloadQueue depends on that status to pick a new DownloadTask
        await _dbContext.DetermineDownloadStatus(notification.Key, cancellationToken);

        var downloadTasks = await _dbContext.GetAllDownloadTasksByServerAsync(
            plexServerId,
            cancellationToken: cancellationToken
        );

        // Update the front-end with the download progress
        await _signalRService.SendDownloadProgressUpdateAsync(downloadTasks, cancellationToken);

        var changedDownloadTask = await _dbContext.GetDownloadTaskAsync(notification.Key, cancellationToken);
        if (changedDownloadTask is null)
        {
            ResultExtensions.EntityNotFound(nameof(DownloadTaskGeneric), notification.Key.ToString()).LogError();
            return;
        }

        if (changedDownloadTask.DownloadStatus == DownloadStatus.DownloadFinished)
        {
            var addFileTaskResult = await _fileMergeScheduler.CreateFileTaskFromDownloadTask(notification.Key);
            if (addFileTaskResult.IsFailed)
            {
                addFileTaskResult.LogError();
                return;
            }

            await _fileMergeScheduler.StartFileMergeJob(addFileTaskResult.Value.Id);
            await _mediator.Publish(
                new CheckDownloadQueueNotification(notification.Key.PlexServerId),
                cancellationToken
            );
        }
    }
}
