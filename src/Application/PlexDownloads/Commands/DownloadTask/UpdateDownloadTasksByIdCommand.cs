﻿namespace PlexRipper.Application;

public class UpdateDownloadTasksByIdCommand : IRequest<Result>
{
    public List<DownloadTask> DownloadTasks { get; }

    public UpdateDownloadTasksByIdCommand(List<DownloadTask> downloadTasks)
    {
        DownloadTasks = downloadTasks;
    }
}