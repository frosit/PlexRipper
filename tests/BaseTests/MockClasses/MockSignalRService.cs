﻿using System.Collections.Concurrent;
using Application.Contracts;
using Logging.Interface;
using WebAPI.Contracts;

namespace PlexRipper.BaseTests;

public class MockSignalRService : ISignalRService
{
    private readonly ILog<MockSignalRService> _log;

    public BlockingCollection<DownloadTaskDTO> DownloadTaskUpdate { get; } = new();

    public BlockingCollection<FileMergeProgress> FileMergeProgressList { get; } = new();

    public BlockingCollection<ServerDownloadProgressDTO> ServerDownloadProgressList { get; } = new();
    public BlockingCollection<JobStatusUpdateDTO> JobStatusUpdateList { get; } = new();
    public BlockingCollection<DataType> RefreshNotificationList { get; } = new();

    public MockSignalRService(ILog<MockSignalRService> log)
    {
        _log = log;
    }

    public Task SendLibraryProgressUpdateAsync(LibraryProgress libraryProgress) => Task.CompletedTask;

    public Task SendLibraryProgressUpdateAsync(int id, int received, int total, bool isRefreshing = true) =>
        Task.CompletedTask;

    public Task SendDownloadTaskCreationProgressUpdate(int current, int total) => Task.CompletedTask;

    public Task SendFileMergeProgressUpdateAsync(
        FileMergeProgress fileMergeProgress,
        CancellationToken cancellationToken = default
    )
    {
        FileMergeProgressList.Add(fileMergeProgress, cancellationToken);
        _log.Verbose("{ClassName} => {@FileMergeProgress}", nameof(MockSignalRService), fileMergeProgress);
        return Task.CompletedTask;
    }

    public Task SendNotificationAsync(Notification notification) => Task.CompletedTask;

    public Task SendServerSyncProgressUpdateAsync(SyncServerMediaProgress syncServerMediaProgress) =>
        Task.CompletedTask;

    public Task SendDownloadProgressUpdateAsync(
        List<DownloadTaskGeneric> downloadTasks,
        CancellationToken cancellationToken = default
    )
    {
        var update = downloadTasks.ToServerDownloadProgressDTOList();

        ServerDownloadProgressList.Add(update.First(), cancellationToken);
        _log.Verbose("{ClassName} => {@DownloadTaskDto}", nameof(MockSignalRService), update);

        return Task.CompletedTask;
    }

    public Task SendServerConnectionCheckStatusProgressAsync(ServerConnectionCheckStatusProgress progress) =>
        Task.CompletedTask;

    public Task SendJobStatusUpdateAsync<T>(JobStatusUpdate<T> jobStatusUpdate)
        where T : class
    {
        JobStatusUpdateList.Add(jobStatusUpdate.ToDTO());
        _log.Verbose("{ClassName} => {@JobStatusUpdate}", nameof(MockSignalRService), jobStatusUpdate);

        return Task.CompletedTask;
    }

    public Task SendRefreshNotificationAsync(DataType dataType, CancellationToken cancellationToken = default)
    {
        RefreshNotificationList.Add(dataType, cancellationToken);
        _log.Verbose("{ClassName} => {@DataType}", nameof(MockSignalRService), dataType);

        return Task.CompletedTask;
    }

    public async Task SendRefreshNotificationAsync(
        List<DataType> dataTypes,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var dataType in dataTypes)
            await SendRefreshNotificationAsync(dataType, cancellationToken);
    }
}
