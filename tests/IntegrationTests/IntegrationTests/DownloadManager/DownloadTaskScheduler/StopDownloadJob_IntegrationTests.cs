using Data.Contracts;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.DownloadManager.DownloadTaskScheduler;

public class StopDownloadJob_IntegrationTests : BaseIntegrationTests
{
    public StopDownloadJob_IntegrationTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task ShouldStartAndStopDownloadJob_WhenDownloadTaskHasBeenStopped()
    {
        // Arrange
        Seed = 45644875;
        var serverUri = SpinUpPlexServer(config => { config.DownloadFileSizeInMb = 50; });
        await SetupDatabase(config =>
        {
            config.MockServerUris.Add(serverUri);
            config.PlexAccountCount = 1;
            config.PlexServerCount = 1;
            config.PlexLibraryCount = 3;
            config.MovieCount = 10;
            config.MovieDownloadTasksCount = 1;
            config.DownloadFileSizeInMb = 50;
        });

        SetupMockPlexApi();

        await CreateContainer(config => { config.DownloadSpeedLimitInKib = 5000; });

        var movieDownloadTasks = await DbContext.DownloadTaskMovie.Include(x => x.Children).ToListAsync();

        var childDownloadTask = movieDownloadTasks[0].Children[0];

        // Act
        var startResult = await Container.DownloadTaskScheduler.StartDownloadTaskJob(childDownloadTask.ToKey());
        await Task.Delay(500);
        var stopResult = await Container.DownloadTaskScheduler.StopDownloadTaskJob(childDownloadTask.ToKey());
        await Container.SchedulerService.AwaitScheduler();

        // Assert
        startResult.IsSuccess.ShouldBeTrue(startResult.ToString());
        stopResult.IsSuccess.ShouldBeTrue(stopResult.ToString());
        var downloadTaskDb = await IDbContext.GetDownloadTaskAsync(childDownloadTask.ToKey());
        downloadTaskDb.ShouldNotBeNull();
        downloadTaskDb.DownloadStatus.ShouldBe(DownloadStatus.Stopped);
    }
}