using LukeHagar.PlexAPI.SDK.Utils;
using PlexRipper.Domain;

namespace PlexApi.Contracts;

public interface IPlexApiClient : IDisposable, ISpeakeasyHttpClient
{
    Task<ThrottledStream?> DownloadStreamAsync(
        HttpRequestMessage request,
        int downloadSpeedLimit,
        CancellationToken cancellationToken
    );
}
